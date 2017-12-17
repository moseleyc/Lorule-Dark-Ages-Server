using System;
using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;

namespace Darkages.Storage.locales.Scripts.Monsters
{
    [Script("Common Magical Monster", "Dean")]
    public class CommonMagicalMonster : MonsterScript
    {
        public CommonMagicalMonster(Monster monster, Area map)
            : base(monster, map)
        {
        }

        public override void OnApproach(GameClient client)
        {
            if (!Monster.WithinRangeOf(client.Aisling))
            {
                Monster.Target = null;
                return;
            }

            if (Monster.Target == null && Monster.WithinRangeOf(client.Aisling) && !client.Aisling.Invisible)
                Monster.Target = client.Aisling;
        }

        public override void OnAttacked(GameClient client)
        {
        }

        public override void OnCast(GameClient client)
        {
        }

        public override void OnClick(GameClient client)
        {
            Monster.Attacked = true;
            Monster.Target = client.Aisling;

            client.SendMessage(0x02,
                Monster.Template.Name +
                $"(Lv {Monster.Template.Level}, HP: {Monster.CurrentHp}/{Monster.MaximumHp}, AC: {Monster.Ac})");
        }

        public override void OnDeath(GameClient client)
        {
            if (Monster.Target != null)
                if (Monster.Target is Aisling)
                    Monster.GiveExperienceTo(Monster.Target as Aisling);

            if (GetObject<Monster>(i => i.Serial == Monster.Serial) != null)
                DelObject(Monster);
        }

        public override void OnLeave(GameClient client)
        {
            Monster.Target = null;
        }

        public override void Update(TimeSpan elapsedTime)
        {
            if (Monster != null && Monster.isAlive)
            {
                Monster.WalkTimer.Update(elapsedTime);
                Monster.BashTimer.Update(elapsedTime);

                if (Monster.Target != null)
                    if (Monster.Target is Aisling)
                        if ((Monster.Target as Aisling).Invisible)
                            Monster.Target = null;

                if (Monster.BashTimer.Elapsed)
                {
                    if (Monster.BashEnabled)
                        if (Monster.Target == null)
                        {
                            Monster.BashEnabled = false;
                        }
                        else
                        {
                            int direction;

                            if (Monster.Facing(Monster.Target.X, Monster.Target.Y, out direction))
                            {
                                Monster.Attack();
                            }
                            else
                            {
                                if (!Monster.Facing(Monster.Target.X, Monster.Target.Y, out direction))
                                {
                                    Monster.Direction = (byte) direction;
                                    Monster.Turn();
                                }
                            }
                        }

                    Monster.BashTimer.Reset();
                }

                if (Monster.WalkTimer.Elapsed)
                {
                    if (Monster.WalkEnabled && !Monster.Attacked)
                    {
                        Monster.Wander();
                    }
                    else
                    {
                        if (Monster.Target != null)
                            if (Monster.NextTo(Monster.Target.X, Monster.Target.Y))
                            {
                                int direction;

                                if (Monster.Facing(Monster.Target.X, Monster.Target.Y, out direction))
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
                            else
                            {
                                int direction;

                                if (!Monster.Facing(Monster.Target.X, Monster.Target.Y, out direction))
                                {
                                    Monster.Direction = (byte) direction;
                                    Monster.Turn();
                                }

                                Monster.BashEnabled = false;
                                Monster.CastEnabled = false;

                                Monster.WalkTo(Monster.Target.X, Monster.Target.Y);
                            }
                    }
                    Monster.WalkTimer.Reset();
                }
            }
        }
    }
}