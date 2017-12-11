using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Newtonsoft.Json;
using System;
using System.ComponentModel;

namespace Darkages.Types
{
    public class Monster : Sprite
    {
        [JsonIgnore]
        public MonsterScript Script { get; private set; }

        public GameServerTimer BashTimer { get; set; }
        public GameServerTimer CastTimer { get; set; }
        public GameServerTimer WalkTimer { get; set; }
        public MonsterTemplate Template { get; set; }

        public bool Attacked { get; set; }
        public bool isAlive { get; set; }

        [Browsable(false)]
        public bool BashEnabled { get; set; }
        [Browsable(false)]
        public bool CastEnabled { get; set; }
        [Browsable(false)]
        public bool WalkEnabled { get; set; }

        public ushort Image { get; private set; }

        [JsonIgnore]
        [Browsable(false)]
        public Sprite Target { get;  set; }

        [JsonIgnore]
        public bool GivenExp { get; set; }
    
        public bool Aggressive { get; set; }
        public bool Friendly { get; set; }
        public bool Helper { get; set; }

        public Monster()
        {
            BashEnabled = false;
            CastEnabled = false;
            WalkEnabled = false;

            isAlive = true;
            Attacked = false;
        }

        public bool NextTo(int x, int y)
        {
            int xDist = Math.Abs(x - this.X);
            int yDist = Math.Abs(y - this.Y);

            return (xDist + yDist) == 1;
        }

        public bool NextTo(Sprite target) => NextTo(target.X, target.Y);
        public bool NextToTarget() => NextTo(Target);
        public void WalkToTarget() => WalkToTarget(Target);
        public void WalkToTarget(Sprite target) => WalkTo(target.X, target.Y);

        public Random rnd = new Random();

        public void GiveExperienceTo(Aisling player)
        {
            if (!GivenExp)
            {
                Money.Create(this, rnd.Next(100, 2000), new Position(X, Y));
                GivenExp = true;

                if (player == null)
                    return;


                if (player.Client.Aisling == null)
                    return;

                var percent = 0.3;
                var poly = 9;
                var coponent = poly + player.ExpLevel / this.Template.Level + 99 * 2;
                var exp_to_award = Math.Round(((coponent / percent) * (player.ExpLevel * 0.30)));
                var exp_gained = Math.Round(player.ExpLevel * exp_to_award);

                player.ExpTotal += (int)(exp_to_award);
                player.ExpNext -= (int)(exp_to_award);

                player.Client.SendMessage(0x02, string.Format("You received {0} Experience!.", exp_to_award));

                if (player.ExpNext <= 0)
                {
                    player.ExpNext = (int)exp_gained * (int)(player.ExpLevel * 0.45) / 6;
                    player._MaximumHp += (int)((50 * player.Con) * 0.65);
                    player._MaximumMp += (int)((25 * player.Wis) * 0.45);
                    player.StatPoints += 2;
                    player.ExpLevel++;

                    if (player.ExpLevel > 99)
                    {
                        player.AbpLevel++;
                        player.ExpLevel = 99;
                    }

                    if (player.AbpLevel > 99)
                    {
                        player.AbpLevel = 99;
                        player.GamePoints++;
                    }

                    player.Client.SendMessage(0x02, string.Format("You have reached level {0}!", player.ExpLevel));
                    player.Show(Scope.NearbyAislings, new ServerFormat29((uint)player.Serial, (uint)player.Serial, 0x004F, 0x004F, 64));
                }
                player.Client.Send(new ServerFormat08(player, StatusFlags.StructA | StatusFlags.StructC));
            }
        }

        public void Attack()
        {
            var target = this.Target;

            if (target == null)
                return;

            Attack(target);
        }

        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(new Random().Next(v.Length));
        }

        public static Monster Create(MonsterTemplate template, Area map)
        {
            if (template.CastSpeed == 0)
                template.CastSpeed = 2000;

            if (template.AttackSpeed == 0)
                template.AttackSpeed = 1000;

            if (template.MovementSpeed == 0)
                template.MovementSpeed = 2000;

            if (template.Level == 0)
                template.Level = 1;

            var obj = new Monster();
            obj.Template = template;
            obj.Map = map;
            obj.CastTimer = new GameServerTimer(TimeSpan.FromMilliseconds(template.CastSpeed));
            obj.BashTimer = new GameServerTimer(TimeSpan.FromMilliseconds(template.AttackSpeed));
            obj.WalkTimer = new GameServerTimer(TimeSpan.FromMilliseconds(template.MovementSpeed));
            obj.BashEnabled = true;
            obj.CastEnabled = template.MaximumMP > 0;

            if (obj.Template.Grow)
            {
                obj.Template.Level++;
            }

            if (obj.Template.Level > 99)
                obj.Template.Level = 1;


            //=E4 / 0.1 * E6 
            obj.Template.MaximumHP = (int) ((obj.Template.Level / 0.1) * 32);
            obj.Template.MaximumMP = (int) ((obj.Template.Level / 0.1) * 16);

            //calculate what ac to give depending on level.
            obj.BonusAc = (sbyte) (70 - (101 / 70 * template.Level));

            if (obj.BonusAc > ServerContext.Config.BaseAC)
                obj.BonusAc = ServerContext.Config.BaseAC;

            if (obj.Template.ElementType == ElementQualifer.Random)
            {
                obj.DefenseElement = RandomEnumValue<ElementManager.Element>();
                obj.OffenseElement = RandomEnumValue<ElementManager.Element>();
            }

            obj.BonusMr = (byte) (10 * (((template.Level / 10) * 100) / 100));

            if (obj.BonusMr > 70)
                obj.BonusMr = 70;

            if ((template.PathQualifer & PathQualifer.Wander) == PathQualifer.Wander)
                obj.WalkEnabled = true;

            if ((template.MoodTyle & MoodQualifer.Aggressive) == MoodQualifer.Aggressive)
            {
                obj.Aggressive = true;
            }

            if ((template.MoodTyle & MoodQualifer.Friendly) == MoodQualifer.Friendly)
            {
                obj.Aggressive = false;
                obj.Friendly = true;
            }

            if ((template.MoodTyle & MoodQualifer.Unpredicable) == MoodQualifer.Unpredicable)
            {
                //this monster has a 50% chance of being friendly.
                obj.Aggressive = (Generator.Random.Next(0, 1) == 1);
                obj.Friendly = !obj.Aggressive;
            }

            if ((template.MoodTyle & MoodQualifer.Defensive) == MoodQualifer.Defensive
                && (template.MoodTyle & MoodQualifer.Friendly) == MoodQualifer.Friendly)
            {
                //AI set to Help.
                obj.Helper = true;
                obj.Aggressive = false;
                obj.Friendly = true;
            }

            if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
            {
                var X = Generator.Random.Next(1, map.Cols);
                var Y = Generator.Random.Next(1, map.Rows);

                while (map.IsWall(obj, X, Y) || map.Tile[X, Y] != TileContent.None)
                {
                    X = Generator.Random.Next(1, map.Cols);
                    Y = Generator.Random.Next(1, map.Rows);
                }

                obj.X = X;
                obj.Y = Y;
            }
            else if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
            {
                obj.X = template.DefinedX;
                obj.Y = template.DefinedY;

                var invalid = false;
                //if not available. find a nearbly location nearby and try spawn it there.
                while (map.IsWall(obj, obj.X, obj.Y) || map.Tile[obj.X, obj.Y] != TileContent.None)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        var tiles = map.GetNearByTiles(obj, (short) obj.X, (short) obj.Y, 20);

                        if (tiles.Length > 0)
                        {
                            var pos = tiles[i];
                            if (pos != null)
                            {
                                obj.X = pos.X;
                                obj.Y = pos.Y;
                                break;
                            }
                        }
                        else
                        {
                            invalid = true;
                        }
                    }

                    if (invalid)
                        break;
                }
            }
            else if ((template.SpawnType & SpawnQualifer.Reactor) == SpawnQualifer.Reactor)
            {
                //TODO reactor handling.
                //assign reactor script. ect.
            }

            lock (Generator.Random)
            {
                obj.Serial = Generator.GenerateNumber();
            }
            obj.isAlive = true;
            obj.CurrentMapId = map.ID;
            obj.CurrentHp = template.MaximumHP;
            obj.CurrentMp = template.MaximumMP;
            obj._MaximumHp = template.MaximumHP;
            obj._MaximumMp = template.MaximumMP;
            obj.CreationDate = DateTime.UtcNow;
            obj.Image = template.ImageVarience
                        > 0
                ? (ushort) Generator.Random.Next(template.Image, template.Image + template.ImageVarience)
                : template.Image;
            obj.Script = ScriptManager.Load<MonsterScript>(template.ScriptName, obj, map);


            //TODO apply formulas.


            return obj;
        }
    }
}
