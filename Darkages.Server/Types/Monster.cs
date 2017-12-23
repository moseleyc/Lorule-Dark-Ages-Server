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

        [JsonIgnore]
        public bool Attacked => CurrentHp < MaximumHp && IsAlive;

        [JsonIgnore]
        public bool IsAlive => CurrentHp > 0;

        [Browsable(false)]
        public bool BashEnabled { get; set; }
        [Browsable(false)]
        public bool CastEnabled { get; set; }
        [Browsable(false)]
        public bool WalkEnabled { get; set; }

        public ushort Image { get; private set; }

        [JsonIgnore]
        public bool Rewarded { get; set; }
    
        public bool Aggressive { get; set; }

        public Monster()
        {
            BashEnabled = false;
            CastEnabled = false;
            WalkEnabled = false;

        }

        public bool NextTo(int x, int y)
        {
            var xDist = Math.Abs(x - this.X);
            var yDist = Math.Abs(y - this.Y);

            return (xDist + yDist) == 1;
        }

        public bool NextTo(Sprite target) => NextTo(target.X, target.Y);
        public bool NextToTarget() => NextTo(Target);
        public void WalkToTarget() => WalkToTarget(Target);
        public void WalkToTarget(Sprite target) => WalkTo(target.X, target.Y);

        public void GenerateRewards(Aisling player)
        {
            if (Rewarded)
                return;

            if (player.Equals(null))
                return;

            if (player.Client.Aisling == null)
                return;

            {
                GenerateExperience(player);
                GenerateGold();
                GenerateDrops();
            }

            Rewarded = true;
            player.UpdateStats();
        }

        private void GenerateExperience(Aisling player)
        {
            var percent = 0.3;
            var poly = 9;
            var coponent = poly + player.ExpLevel / Template.Level + 99 * 2;
            var expToAward = Math.Round(((coponent / percent) * (player.ExpLevel * 0.30)));
            var expGained = Math.Round(player.ExpLevel * expToAward);

            var p = (player.ExpLevel - Template.Level);

            if (p / 10 > 0)
                expGained = 1;
            else
                expGained = Math.Abs(expToAward);

            if (p < 0)
            {
                expGained = expToAward * (Math.Abs(p) + 3);
            }


            player.ExpTotal += (int)(expGained);
            player.ExpNext -= (int)(expGained);

            player.Client.SendMessage(0x02, string.Format("You received {0} Experience!.", (int)expGained));

            if (player.ExpNext <= 0)
            {
                player.ExpNext = ((int)player.ExpTotal * (int)(player.ExpLevel * 0.45) / 6);
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
        }

        private void GenerateGold()
        {
            lock (rnd)
            {
                if ((Template.LootType & LootQualifer.Gold) == LootQualifer.Gold)
                    Money.Create(this, rnd.Next(
                            (int)Math.Sqrt(Template.Level * 500) / 2,
                            (int)Math.Sqrt(Template.Level * 5000) / 2),
                        new Position(X, Y));
            }
        }

        private void GenerateDrops()
        {
            if (Template.Drops.Count > 0)
            {
                lock (rnd)
                {
                    var idx = rnd.Next(Template.Drops.Count);
                    var rndSelector = Template.Drops[idx];

                    if (ServerContext.GlobalItemTemplateCache.ContainsKey(rndSelector))
                    {
                        var item = Item.Create(this, ServerContext.GlobalItemTemplateCache[rndSelector], true);
                        if (rnd.NextDouble() <= item.Template.DropRate)
                            item.Release(this, Position);
                    }
                }
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
            Random rnd = new Random();

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

            if (obj.BonusMr > ServerContext.Config.BaseMR)
                obj.BonusMr = ServerContext.Config.BaseMR;

            if ((template.PathQualifer & PathQualifer.Wander) == PathQualifer.Wander)
                obj.WalkEnabled = true;

            if ((template.MoodTyle & MoodQualifer.Aggressive) == MoodQualifer.Aggressive)
            {
                obj.Aggressive = true;
            }

            if ((template.MoodTyle & MoodQualifer.Friendly) == MoodQualifer.Friendly)
            {
                obj.Aggressive = false;
            }

            if ((template.MoodTyle & MoodQualifer.Unpredicable) == MoodQualifer.Unpredicable)
            {
                //this monster has a 50% chance of being aggressive.
                obj.Aggressive = (Generator.Random.Next(0, 1) == 1);
            }

            if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
            {
                var x = rnd.Next(1, map.Cols);
                var y = rnd.Next(1, map.Rows);

                while (map.IsWall(obj, x, y) || map.Tile[x, y] != TileContent.None)
                {
                    lock (rnd)
                    {
                        x = rnd.Next(1, map.Cols);
                        y = rnd.Next(1, map.Rows);
                    }
                }

                obj.X = x;
                obj.Y = y;
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

            lock (Generator.Random)
            {
                obj.Serial = Generator.GenerateNumber();
            }

            obj.CurrentMapId = map.ID;
            obj.CurrentHp = template.MaximumHP;
            obj.CurrentMp = template.MaximumMP;
            obj._MaximumHp = template.MaximumHP;
            obj._MaximumMp = template.MaximumMP;
            obj.CreationDate = DateTime.UtcNow;
            obj.Image = template.ImageVarience
                        > 0
                ? (ushort) rnd.Next(template.Image, template.Image + template.ImageVarience)
                : template.Image;
            obj.Script = ScriptManager.Load<MonsterScript>(template.ScriptName, obj, map);



            //TODO apply formulas.


            return obj;
        }
    }
}
