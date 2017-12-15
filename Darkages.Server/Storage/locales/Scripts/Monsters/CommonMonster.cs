using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;
using System;
using System.Collections.Generic;

namespace Darkages.Storage.locales.Scripts.Monsters
{
    [Script("Common Monster", "Dean")]
    public class CommonMonster : MonsterScript
    {
        public List<SpellScript> SpellScripts = new List<SpellScript>();
        public List<SkillScript> SkillScripts = new List<SkillScript>();

        private readonly Random random = new Random();

        public Sprite Target => Monster.Target;

        public CommonMonster(Monster monster, Area map)
            : base(monster, map)
        {
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

        public override void OnApproach(GameClient client)
        {
            if (client.Aisling.Dead)
                return;

            if (client.Aisling.Invisible)
                return;

            if (client.Aisling.Flags.HasFlag(AislingFlags.GM))
                return;

            if (Monster.Aggressive)
            {
                Monster.Attacked = true;
                Monster.Target = client.Aisling;
            }
        }

        public override void OnAttacked(GameClient client)
        {
            if (client.Aisling.Dead)
                return;

            if (!Monster.Friendly)
            {
                Monster.Attacked = true;
                Monster.Target = client.Aisling;
            }
        }

        public override void OnCast(GameClient client)
        {
            if (client.Aisling.Dead)
                return;

            Monster.Attacked = true;
            Monster.Target = client.Aisling;
        }

        public override void OnClick(GameClient client)
        {            
            client.SendMessage(0x02, Monster.Template.Name + $"(Lv {Monster.Template.Level}, HP: {Monster.CurrentHp}/{Monster.MaximumHp}, AC: {Monster.Ac}, O: {Monster.OffenseElement}, D: {Monster.DefenseElement})");
        }

        public override void OnDeath(GameClient client)
        {
            if (Monster.Target != null)
            {
                if (Monster.Target is Aisling)
                    Monster.GiveExperienceTo(Monster.Target as Aisling);
            }

            if (Monster.Template.Drops.Count > 0)
            {
                var idx = random.Next(Monster.Template.Drops.Count);
                var rndSelector = Monster.Template.Drops[idx];

                if (ServerContext.GlobalItemTemplateCache.ContainsKey(rndSelector))
                {
                    var item = Item.Create(Monster, ServerContext.GlobalItemTemplateCache[rndSelector], true);
                    if (random.NextDouble() <= item.Template.DropRate)
                        item.Release(Monster, Monster.Position);
                }

            }

            if (GetObject<Monster>(i => i.Serial == Monster.Serial) != null)
                DelObject<Monster>(Monster);

        }

        public override void OnLeave(GameClient client)
        {
            Monster.Attacked = false;
            Monster.Target = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (!this.Monster.isAlive)
            {
                return;
            }

            if (Monster.Target != null)
            {
                if (Monster.Target is Aisling)
                {
                    if ((Monster.Target as Aisling).Invisible)
                        ClearTarget();
                }
                if (Monster.Target?.CurrentHp == 0)
                {
                    ClearTarget();
                }
            }

            this.Monster.BashTimer.Update(elapsedTime);
            this.Monster.CastTimer.Update(elapsedTime);
            this.Monster.WalkTimer.Update(elapsedTime);


            if (this.Monster.BashTimer.Elapsed)
            {
                this.Monster.BashTimer.Reset();

                if (this.Monster.BashEnabled)
                {
                    this.Bash();
                }
            }

            if (this.Monster.CastTimer.Elapsed)
            {
                this.Monster.CastTimer.Reset();

                if (this.Monster.CastEnabled)
                {
                    this.CastSpell();
                }
            }

            if (this.Monster.WalkTimer.Elapsed)
            {
                this.Monster.WalkTimer.Reset();

                if (this.Monster.WalkEnabled)
                {
                    this.Walk();
                }
            }
        }

        private void CastSpell()
        {
            if (Monster != null && Monster.Target != null)
            {
                if (random.Next(1, 101) < ServerContext.Config.MonsterSpellSuccessRate)
                {
                    var spellidx = random.Next(SpellScripts.Count);
                    SpellScripts[spellidx].OnUse(Monster, Target);
                }
            }
        }

        private void Walk()
        {
            if (this.Monster.Attacked)
            {
                if (this.Target != null)
                {
                    if (this.Monster.NextTo(Target.X, Target.Y))
                    {
                        int direction;

                        if (this.Monster.Facing(Target.X, Target.Y, out direction))
                        {
                            this.Monster.BashEnabled = true;
                            this.Monster.CastEnabled = true;
                        }
                        else
                        {
                            this.Monster.BashEnabled = false;
                            this.Monster.CastEnabled = true;
                            this.Monster.Direction = (byte)direction;
                            this.Monster.Turn();
                        }
                    }
                    else
                    {
                        this.Monster.BashEnabled = false;
                        this.Monster.CastEnabled = true;
                        this.Monster.WalkTo(Target.X, Target.Y);
                    }
                }
            }
            else
            {
                this.Monster.BashEnabled = false;
                this.Monster.CastEnabled = false;
                this.Monster.Wander();
            }
        }

        private void Bash()
        {
            var obj = Monster.GetInfront(1);

            if (obj == null)
                return;

            if (obj.Count == 0 || obj.Find(o => o != null && 
                o.Serial == Monster?.Target?.Serial) == null)
            {
                ClearTarget();
            }

            if (Target != null)
            {
                if (Monster != null && Monster.Target != null)
                {
                    var idx = random.Next(SkillScripts.Count);

                    if (random.Next(1, 101) < ServerContext.Config.MonsterSkillSuccessRate)
                    {
                        SkillScripts[idx].OnUse(Monster);
                    }
                    Monster.Attack(Target);
                }
            }
        }

        private void ClearTarget()
        {
            Monster.CastEnabled = false;
            Monster.BashEnabled = false;
            Monster.WalkEnabled = true;
            Monster.Attacked = false;
            Monster.Target = null;
        }
    }
}

