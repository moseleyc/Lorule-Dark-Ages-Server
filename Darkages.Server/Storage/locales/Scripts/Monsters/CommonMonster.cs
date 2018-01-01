using System;
using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Monsters
{
    [Script("Common Monster", "Dean")]
    public class CommonMonster : MonsterScript
    {
        private readonly Random random = new Random();
        public List<SkillScript> SkillScripts = new List<SkillScript>();
        public List<SpellScript> SpellScripts = new List<SpellScript>();

        public CommonMonster(Monster monster, Area map)
            : base(monster, map)
        {
            if (Monster.Template.SpellScripts != null)
                foreach (var spellscriptstr in Monster.Template.SpellScripts)
                {
                    var script = ScriptManager.Load<SpellScript>(spellscriptstr,
                        Spell.Create(1, ServerContext.GlobalSpellTemplateCache[spellscriptstr]));

                    SpellScripts.Add(script);
                }


            var wff = ScriptManager.Load<SkillScript>("wff",
                Skill.Create(1, ServerContext.GlobalSkillTemplateCache["Wolf Fang Fist"]));

            SkillScripts.Add(wff);
        }

        public Sprite Target => Monster.Target;

        public override void OnApproach(GameClient client)
        {
            if (client.Aisling.Dead)
                return;

            if (client.Aisling.Invisible)
                return;


            if (Monster.Aggressive)
            {
                Monster.Target = client.Aisling;
            }
        }

        public override void OnAttacked(GameClient client)
        {
            if (client.Aisling.Dead)
                return;

            Monster.Target = client.Aisling;
        }

        public override void OnCast(GameClient client)
        {
            if (client.Aisling.Dead)
                return;

            Monster.Target = client.Aisling;
        }

        public override void OnClick(GameClient client)
        {
            client.SendMessage(0x02,
                Monster.Template.Name +
                $"(Lv {Monster.Template.Level}, HP: {Monster.CurrentHp}/{Monster.MaximumHp}, AC: {Monster.Ac}, O: {Monster.OffenseElement}, D: {Monster.DefenseElement})");
        }

        public override void OnDeath(GameClient client)
        {
            if (Monster.Target != null)
                if (Monster.Target is Aisling)
                    Monster.GenerateRewards(Monster.Target as Aisling);


            if (GetObject<Monster>(i => i.Serial == Monster.Serial) != null)
                DelObject(Monster);
        }

        public override void OnLeave(GameClient client)
        {
            Monster.Target = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (!Monster.IsAlive)
                return;

            UpdateTarget();

            Monster.BashTimer.Update(elapsedTime);
            Monster.CastTimer.Update(elapsedTime);
            Monster.WalkTimer.Update(elapsedTime);

            if (Monster.BashTimer.Elapsed)
            {
                Monster.BashTimer.Reset();

                if (Monster.BashEnabled)
                    Bash();
            }

            if (Monster.CastTimer.Elapsed)
            {
                Monster.CastTimer.Reset();

                if (Monster.CastEnabled)
                    CastSpell();
            }

            if (Monster.WalkTimer.Elapsed)
            {
                Monster.WalkTimer.Reset();

                if (Monster.WalkEnabled)
                    Walk();
            }
        }

        private void UpdateTarget()
        {
            if (Monster.Target != null)
            {
                if (Monster.Target is Aisling)
                    if (((Aisling) Monster.Target).Invisible)
                        ClearTarget();

                if (Monster.Target?.CurrentHp == 0)
                    ClearTarget();
            }
        }

        private void CastSpell()
        {
            if (Monster != null && Monster.Target != null && SpellScripts.Count > 0)
                if (random.Next(1, 101) < ServerContext.Config.MonsterSpellSuccessRate)
                {
                    var spellidx = random.Next(SpellScripts.Count);
                    SpellScripts[spellidx].OnUse(Monster, Target);
                }
        }

        private void Walk()
        {
            if (Target != null)
            {
                if (Monster.NextTo(Target.X, Target.Y))
                {
                    lock (Monster)
                    {
                        if (Monster.Facing(Target.X, Target.Y, out var direction))
                        {
                            Monster.BashEnabled = true;
                            Monster.CastEnabled = true;
                        }
                        else
                        {
                            Monster.BashEnabled = false;
                            Monster.CastEnabled = true;
                            Monster.Direction = (byte) direction;
                            Monster.Turn();
                        }
                    }
                }
                else
                {
                    Monster.BashEnabled = false;
                    Monster.CastEnabled = true;
                    Monster.WalkTo(Target.X, Target.Y);
                }
            }
            else
            {
                Monster.BashEnabled = false;
                Monster.CastEnabled = false;
                Monster.Wander();
            }
        }

        private void Bash()
        {
            var obj = Monster.GetInfront(1);

            if (obj == null)
                return;

            if (obj.Count == 0 || obj.Find(o => o != null &&
                                                o.Serial == Monster?.Target?.Serial) == null)
                ClearTarget();

            if (Target == null)
                return;

            if (Monster != null && Monster.Target != null && SkillScripts.Count > 0)
            {
                var idx = random.Next(SkillScripts.Count);

                if (random.Next(1, 101) < ServerContext.Config.MonsterSkillSuccessRate)
                    SkillScripts[idx].OnUse(Monster);
            }
            Monster?.Attack(Target);
        }

        private void ClearTarget()
        {
            Monster.CastEnabled = false;
            Monster.BashEnabled = false;
            Monster.WalkEnabled = true;
            Monster.Target = null;
        }
    }
}