using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Newtonsoft.Json;

namespace Darkages.Types
{
    public class Mundane : Sprite
    {
        public MundaneTemplate Template { get; set; }

        [JsonIgnore]
        public MundaneScript Script { get; set; }

        public static void Create(MundaneTemplate template)
        {
            if (template == null)
                return;

            var existing = template.GetObject<Mundane>(p => p.Template != null && p.Template.Name == template.Name);

            //this npc was already created?
            if (existing != null)
            {
                //check if it's dead.
                if (existing.CurrentHp == 0)
                {
                    existing.OnDeath();
                }
                else
                {
                    //it's alive. no need to re-add.
                    return;
                }
            }

      
            var npc = new Mundane();            
            npc.Template = template;

            npc.CurrentMapId = npc.Template.AreaID;
            lock (Generator.Random)
            {
                npc.Serial = Generator.GenerateNumber();
            }
            npc.X = template.X;
            npc.Y = template.Y;
            npc._MaximumHp = (int)(npc.Template.Level / 0.1 * 32);
            npc._MaximumMp = (int)(npc.Template.Level / 0.1 * 16);
            npc.Template.MaximumHp = npc.MaximumHp;
            npc.Template.MaximumMp = npc.MaximumMp;

            npc.CurrentHp = npc.Template.MaximumHp;
            npc.CurrentMp = npc.Template.MaximumMp;
            npc.Direction = npc.Template.Direction;

            npc.Map = ServerContext.GlobalMapCache[npc.CurrentMapId];
            npc.Script = ScriptManager.Load<MundaneScript>(template.ScriptKey, ServerContext.Game, npc);

            if (npc.Template.EnableAttacking)
            {
                npc.Template.AttackTimer = new GameServerTimer(TimeSpan.FromMilliseconds(450));
            }


            if (npc.Template.EnableWalking)
            {
                npc.Template.EnableTurning = false;
                npc.Template.WalkTimer = new GameServerTimer(TimeSpan.FromSeconds(750));
            }

            if (npc.Template.EnableSpeech)
            {
                npc.Template.ChatTimer = new GameServerTimer(TimeSpan.FromSeconds(Generator.Random.Next(10, 35)));
            }

            if (npc.Template.EnableTurning)
            {
                npc.Template.TurnTimer = new GameServerTimer(TimeSpan.FromSeconds(6));
            }


            npc.AddObject(npc);
        }

        public void OnDeath()
        {
            if (ServerContext.GlobalMapCache.ContainsKey(CurrentMapId))
            {
                ServerContext.GlobalMapCache[CurrentMapId].Tile[X, Y] = TileContent.None;
            }

            Template.EnableWalking = false;

            RemoveActiveTargets();

            if (CurrentHp == 0)
            {
                new TaskFactory().StartNew(() =>
                {
                    Template.WalkTimer = null;
                    Template.AttackTimer = null;

                    Thread.Sleep(1000);
                    Remove<Mundane>();
                });
            }
        }

        private void RemoveActiveTargets()
        {
            var nearbyMonsters = GetObjects<Monster>(i => WithinRangeOf(this));
            foreach (var nearby in nearbyMonsters)
            {
                if (nearby.Target != null && nearby.Target.Serial == Serial)
                {
                    nearby.Target = null;
                    SaveObject(nearby);
                }
            }
        }

        public void Update(TimeSpan update)
        {
            if (Template == null)
                return;

            SaveObject(this);

            if (Template.ChatTimer != null)
            {
                Template.ChatTimer.Update(update);

                if (Template.ChatTimer.Elapsed)
                {
                    var nearby = GetObjects<Aisling>(i => i.WithinRangeOf(this));
                    var idx = 0;
                    foreach (var obj in nearby)
                    {
                        lock (Generator.Random)
                        {
                            idx = Generator.Random.Next(Template.Speech.Count);
                        }

                        obj.Show(Scope.Self, new ServerFormat0D { Serial = Serial, Text = Template.Name + ": " + Template.Speech[idx], Type = 0x00 });
                    }


                    Template.ChatTimer.Reset();
                }
            }

            if (Template.TurnTimer != null)
            {
                Template.TurnTimer.Update(update);
                if (Template.TurnTimer.Elapsed)
                {
                    lock (Generator.Random)
                    {
                        Direction = (byte)(Generator.Random.Next(0, 4));
                    }
                    Turn();

                    Template.TurnTimer.Reset();
                }
            }

            if (Template.AttackTimer != null && Template.EnableWalking)
            {
                Template.AttackTimer.Update(update);
                if (Template.AttackTimer.Elapsed)
                {
                    var targets = GetObjects<Monster>(i => i.WithinRangeOf(this)).OrderBy(i => i.Position.DistanceFrom(Position));

                    foreach (var t in targets)
                    {
                        t.Target = this;
                    }

                    var target = Target == null ? targets.FirstOrDefault() : Target;

                    if (target?.CurrentHp == 0)
                        target = null;

                    if (target != null)
                    {

                        Script?.TargetAcquired(target);

                        if (!Position.IsNextTo(target.Position))
                        {
                            WalkTo(target.X, target.Y);
                        }
                        else
                        {
                            if (!Facing(target, out var direction))
                            {
                                Direction = (byte)direction;
                                Turn();
                            }
                            else
                            {
                                target.Target = this;
                                Attack(target);
                            }
                        }
                    }
                    else
                    {
                        Wander();
                    }

                    Template.AttackTimer.Reset();
                }
            }
        }
    }
}
