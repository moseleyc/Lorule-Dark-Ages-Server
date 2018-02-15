﻿using Darkages.Common;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Systems.Loot;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using static Darkages.ServerContext;

namespace Darkages.Types
{
    public class Monster : Sprite
    {

        public Monster()
        {
            BashEnabled = false;
            CastEnabled = false;
            WalkEnabled = false;
            WaypointIndex = 0;
        }

        [JsonIgnore] public MonsterScript Script { get; set; }

        public GameServerTimer BashTimer { get; set; }
        public GameServerTimer CastTimer { get; set; }
        public GameServerTimer WalkTimer { get; set; }
        public MonsterTemplate Template { get; set; }

        [JsonIgnore] public bool IsAlive => CurrentHp > 0;

        [Browsable(false)] public bool BashEnabled { get; set; }

        [Browsable(false)] public bool CastEnabled { get; set; }

        [Browsable(false)] public bool WalkEnabled { get; set; }

        public ushort Image { get; private set; }

        [JsonIgnore] public bool Rewarded { get; set; }

        public bool Aggressive { get; set; }

        [JsonIgnore]
        public int WaypointIndex = 0;

        [JsonIgnore]
        public Position CurrentWaypoint => Template.Waypoints[WaypointIndex] ?? null;

        [JsonIgnore]
        public LootTable LootTable { get; set; }

        [JsonIgnore]
        public ConcurrentDictionary<int, Sprite> TaggedAislings { get; set; }

        [JsonIgnore]
        public LootTable UpgradeTable { get; set; }


        [JsonIgnore]
        public LootDropper LootManager { get; set; }

        public bool NextTo(int x, int y)
        {
            var xDist = Math.Abs(x - X);
            var yDist = Math.Abs(y - Y);

            return xDist + yDist == 1;
        }

        public bool NextTo(Sprite target)
        {
            return NextTo(target.X, target.Y);
        }

        public void GenerateRewards(Aisling player, bool TestingMode = false)
        {
            if (Rewarded)
                return;

            if (player.Equals(null))
                return;

            if (player.Client.Aisling == null)
                return;

            if (!TestingMode)
            {
                GenerateExperience(player);
                GenerateGold();
            }

            GenerateDrops(TestingMode);

            Rewarded = true;
            player.UpdateStats();
        }

        private void GenerateExperience(Aisling player)
        {
            var percent = 0.3;
            var poly    = 9;

            var coponent   = poly + player.ExpLevel / Template.Level + 99 * 2;
            var expToAward = Math.Round(coponent / percent * (player.ExpLevel * 0.30));
            var expGained  = Math.Round(player.ExpLevel * expToAward);

            unchecked
            {
                expGained = (expGained / player.GroupParty.Length);
            }

            DistributeExperience(player, expGained, expToAward);

            foreach (var party in player.PartyMembers.Where(i => i.Serial != player.Serial))
            {
                if (party.WithinRangeOf(player))
                    DistributeExperience(party, expGained, expToAward);
            }
        }

        private void DistributeExperience(Aisling player, double expGained, double expToAward)
        {
            var p = player.ExpLevel - Template.Level;

            if (p / 10 > 0)
                expGained = 1;


            if (p < 0)
                expGained = expToAward * (Math.Abs(p) + 3);


            player.ExpTotal += (int)expGained;
            player.ExpNext -= (int)expGained;

            player.Client.SendMessage(0x02, string.Format("You received {0} Experience!.", (int)expGained));

            while (player.ExpNext <= 0)
            {
                player.ExpNext = (int)(player.ExpLevel * (1 + (player.ExpLevel * (3 + 2 * player.ExpLevel)) / 6 + expGained));

                Levelup(player);
            }
        }

        private static void Levelup(Aisling player)
        {
            player._MaximumHp += (int)(50 * player.Con * 0.65);
            player._MaximumMp += (int)(25 * player.Wis * 0.45);
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

            player.Client.SendMessage(0x02, string.Format(ServerContext.Config.LevelUpMessage, player.ExpLevel));
            player.Show(Scope.NearbyAislings,
                new ServerFormat29((uint)player.Serial, (uint)player.Serial, 0x004F, 0x004F, 64));
        }

        private void GenerateGold()
        {
            if (!Template.LootType.HasFlag(LootQualifer.Gold))
                return;

            int sum = 0;

            lock (rnd)
            {
                sum = rnd.Next(
                            Template.Level * 500,
                            Template.Level * 1000);
            }

            if (sum > 0)
                Money.Create(this, sum, new Position(X, Y));
        }

        private List<string> DetermineDrop()
            => LootManager.Drop(LootTable, rnd.Next(ServerContext.Config.LootTableStackSize))
                .Select(i => i?.Name).ToList();

        private ItemUpgrade DetermineQuality()
            => (ItemUpgrade)LootManager.Drop(UpgradeTable, 1).FirstOrDefault();

        private void DetermineRandomDrop()
        {
            int idx = 0;
            if (Template.Drops.Count > 0)
                lock (rnd)
                {
                    idx = rnd.Next(Template.Drops.Count);
                }

            var rndSelector = Template.Drops[idx];
            if (GlobalItemTemplateCache.ContainsKey(rndSelector))
            {
                var item = Item.Create(this, GlobalItemTemplateCache[rndSelector], true);
                var chance = 0.00;

                lock (rnd)
                {
                    chance = Math.Round(rnd.NextDouble(), 2);
                }

                if (chance <= item.Template.DropRate)
                    item.Release(this, Position);
            }
        }

        private void GenerateDrops(bool TestMode = false)
        {
            if (Template.LootType.HasFlag(LootQualifer.Table))
            {
                if (LootTable == null || LootManager == null)
                    return;

                lock (rnd)
                {
                    DetermineDrop().ForEach(i =>
                    {
                        if (i != null)
                        {

                            var rolled_item = Item.Create(this, GlobalItemTemplateCache[i]);
                            var upgrade = DetermineQuality();
                            rolled_item.Upgrades = upgrade?.Upgrade ?? 0;
                            Item.ApplyQuality(rolled_item);

#warning  TODO: remove later. testing roll systems.
                            if (TestMode)
                                return;


                            rolled_item.Cursed = true;
                            rolled_item.AuthenticatedAislings = GetTaggedAislings();
                            rolled_item.Release(this, this.Position);

                            if (rolled_item.Upgrades > 3)
                            {
                                var users = GetTaggedAislings();
                                foreach (var user in users)
                                {
                                    var msg = string.Format("{0} Drop!!! ({1})", upgrade?.Name, rolled_item.DisplayName);
                                    user.Client.SendMessage(3, msg);

                                    //TODO: implement more rarity animations to display.
                                    user.Client.SendAnimation(341, rolled_item, rolled_item, 0x64, true);
                                }
                            }
                        }
                    });
                }
                return;
            }
            else if (Template.LootType.HasFlag(LootQualifer.Random))
            {
                DetermineRandomDrop();
                return;
            }
        }

        private Sprite[] GetTaggedAislings()
        {
            var tagged = TaggedAislings.Select(i => i.Value).ToArray();
            return tagged;
        }

        public void Attack()
        {
            var target = Target;

            if (target == null)
                return;

            AppendTags(target);
            Attack(target);
        }

        public void AppendTags(Sprite target)
        {
            TaggedAislings = new ConcurrentDictionary<int, Sprite>();

            if (target is Aisling)
            {
                var aisling = target as Aisling;

                if (!TaggedAislings.ContainsKey(aisling.Serial))
                    TaggedAislings.TryAdd(aisling.Serial, aisling);

                if (aisling.GroupParty.LengthExcludingSelf > 0)
                {
                    foreach (var member in aisling.GroupParty.MembersExcludingSelf)
                    {
                        if (!TaggedAislings.ContainsKey(member.Serial))
                            TaggedAislings.TryAdd(member.Serial, member);
                    }
                }
            }
        }

        private static T RandomEnumValue<T>()
        {
            lock (Generator.Random)
            {
                var v = Enum.GetValues(typeof(T));
                return (T)v.GetValue(Generator.Random.Next(1, v.Length));
            }
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
            obj.CastEnabled = template.MaximumMP > 0;
            obj.TaggedAislings = new ConcurrentDictionary<int, Sprite>();

            if (obj.Template.Grow)
                obj.Template.Level++;

            if (obj.Template.Level > 99)
                obj.Template.Level = 1;


            //=E4 / 0.1 * E6 
            obj.Template.MaximumHP = (int)(obj.Template.Level / 0.1 * 3);
            obj.Template.MaximumMP = (int)(obj.Template.Level / 0.1 * 2);

            //calculate what ac to give depending on level.
            obj.BonusAc = (sbyte)(70 - 101 / 70 * template.Level);

            if (obj.BonusAc > Config.BaseAC)
                obj.BonusAc = Config.BaseAC;

            obj.DefenseElement = ElementManager.Element.None;
            obj.OffenseElement = ElementManager.Element.None;


            if (obj.Template.ElementType == ElementQualifer.Random && obj.Template.Level > 3)
            {
                obj.DefenseElement = RandomEnumValue<ElementManager.Element>();
                obj.OffenseElement = RandomEnumValue<ElementManager.Element>();
            }
            else if (obj.Template.ElementType == ElementQualifer.Defined)
            {
                obj.DefenseElement = template?.DefenseElement == ElementManager.Element.None
                    ? RandomEnumValue<ElementManager.Element>()
                    : template.DefenseElement;
                obj.OffenseElement = template?.OffenseElement == ElementManager.Element.None
                    ? RandomEnumValue<ElementManager.Element>()
                    : template.OffenseElement;
            }

            obj.BonusMr = (byte)(10 * (template.Level / 10 * 100 / 100));

            if (obj.BonusMr > Config.BaseMR)
                obj.BonusMr = Config.BaseMR;

            if ((template.PathQualifer & PathQualifer.Wander) == PathQualifer.Wander)
                obj.WalkEnabled = true;
            else if ((template.PathQualifer & PathQualifer.Fixed) == PathQualifer.Fixed)
                obj.WalkEnabled = false;
            else if ((template.PathQualifer & PathQualifer.Patrol) == PathQualifer.Patrol)
                obj.WalkEnabled = true;


            if (template.MoodTyle == MoodQualifer.Aggressive)
                obj.Aggressive = true;
            else if (template.MoodTyle == MoodQualifer.Unpredicable)
                lock (Generator.Random)
                {
                    //this monster has a 50% chance of being aggressive.
                    obj.Aggressive = Generator.Random.Next(1, 101) > 50;
                }

            if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
            {
                var x = Generator.Random.Next(1, map.Cols);
                var y = Generator.Random.Next(1, map.Rows);

                var tries = 0;
                var success = false;

                //let monters spawn on anything passable. 
                while ((map.IsWall(obj, x, y) || map[x, y] != TileContent.Aisling
                    && map.IsWall(obj, x, y) || map[x, y] != TileContent.Monster
                    && map.IsWall(obj, x, y) || map[x, y] != TileContent.Mundane
                    && map.IsWall(obj, x, y) || map[x, y] != TileContent.Wall) && tries <= 256)
                {
                    lock (Generator.Random)
                    {
                        x = Generator.Random.Next(1, map.Cols);
                        y = Generator.Random.Next(1, map.Rows);
                    }

                    tries++;
                }

                success = (tries < 256);

                if (!success)
                {
                    lock (Generator.Random)
                    {
                        x = Generator.Random.Next(1, map.Cols);
                        y = Generator.Random.Next(1, map.Rows);
                    }
                }

                obj.X = x;
                obj.Y = y;
            }
            else if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
            {
                obj.X = template.DefinedX;
                obj.Y = template.DefinedY;
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
            obj.AbandonedDate = DateTime.UtcNow;

            lock (Generator.Random)
            {
                obj.Image = template.ImageVarience
                            > 0
                    ? (ushort)Generator.Random.Next(template.Image, template.Image + template.ImageVarience)
                    : template.Image;
            }

            obj.Script = ScriptManager.Load<MonsterScript>(template.ScriptName, obj, map);

            if (obj.Template.LootType.HasFlag(LootQualifer.Table))
            {
                obj.LootManager = new LootDropper();
                obj.LootTable = new LootTable(template.Name);
                obj.UpgradeTable = new LootTable("Probabilities");

                foreach (var drop in obj.Template.Drops)
                    obj.LootTable.Add(GlobalItemTemplateCache[drop]);

                obj.UpgradeTable.Add(new Common());
                obj.UpgradeTable.Add(new Uncommon());
                obj.UpgradeTable.Add(new Rare());
                obj.UpgradeTable.Add(new Epic());
                obj.UpgradeTable.Add(new Legendary());
                obj.UpgradeTable.Add(new Mythical());
                obj.UpgradeTable.Add(new Godly());
                obj.UpgradeTable.Add(new Forsaken());
            }

            return obj;
        }

        public void Patrol()
        {
            if (CurrentWaypoint != null)
            {
                WalkTo(CurrentWaypoint.X, CurrentWaypoint.Y);
            }

            if (Position.DistanceFrom(CurrentWaypoint) <= 1 || CurrentWaypoint == null)
            {
                if (WaypointIndex + 1 < Template.Waypoints.Count)
                    WaypointIndex++;
                else
                    WaypointIndex = 0;
            }
        }
    }
}