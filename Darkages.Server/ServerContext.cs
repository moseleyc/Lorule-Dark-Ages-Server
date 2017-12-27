using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using Darkages.Network.Game;
using Darkages.Network.Login;
using Darkages.Network.Object;
using Darkages.Storage;
using Darkages.Types;

namespace Darkages
{
    public class ServerContext : ObjectManager
    {
        public static ServerConstants Config;

        public static int ERRORS;
        public static int DEFAULT_PORT;


        public static List<Redirect> GlobalRedirects = new List<Redirect>();
        public static List<Metafile> GlobalMetaCache = new List<Metafile>();
        public static Dictionary<int, Area> GlobalMapCache = new Dictionary<int, Area>();

        public static Dictionary<string, MonsterTemplate> GlobalMonsterTemplateCache =
            new Dictionary<string, MonsterTemplate>();

        public static Dictionary<string, SkillTemplate> GlobalSkillTemplateCache =
            new Dictionary<string, SkillTemplate>();

        public static Dictionary<string, SpellTemplate> GlobalSpellTemplateCache =
            new Dictionary<string, SpellTemplate>();

        public static Dictionary<string, ItemTemplate> GlobalItemTemplateCache = new Dictionary<string, ItemTemplate>();

        public static Dictionary<string, MundaneTemplate> GlobalMundaneTemplateCache =
            new Dictionary<string, MundaneTemplate>();

        public static Dictionary<int, List<WarpTemplate>> GlobalWarpTemplateCache =
            new Dictionary<int, List<WarpTemplate>>();

        public static bool Running;
        public static IPAddress IPADDRESS => IPAddress.Parse(File.ReadAllText(Config.ServerTablePath));
        public static string STORAGE_PATH => @"..\..\..\Storage\Locales";

        public static GameServer Game { get; set; }
        public static LoginServer Lobby { get; set; }

        public static void LoadSkillTemplates()
        {
            Console.WriteLine("\n----- Loading Skills -----");
            StorageManager.SKillBucket.CacheFromStorage();
            Console.WriteLine(" ... Skill Templates Loaded: {0}", GlobalSkillTemplateCache.Count);
        }

        public static void LoadSpellTemplates()
        {
            Console.WriteLine("\n----- Loading Spells -----");
            StorageManager.SpellBucket.CacheFromStorage();
            Console.WriteLine(" ... Spell Templates Loaded: {0}", GlobalSpellTemplateCache.Count);
        }

        public static void LoadItemTemplates()
        {
            Console.WriteLine("\n----- Loading Items -----");
            StorageManager.ItemBucket.CacheFromStorage();
            Console.WriteLine(" ... Item Templates Loaded: {0}", GlobalItemTemplateCache.Count);
        }

        public static void LoadMonsterTemplates()
        {
            Console.WriteLine("\n----- Loading Monsters -----");
            StorageManager.MonsterBucket.CacheFromStorage();
            Console.WriteLine(" ... Monster Templates Loaded: {0}", GlobalMonsterTemplateCache.Count);
        }

        public static void LoadMundaneTemplates()
        {
            Console.WriteLine("\n----- Loading Mundanes -----");
            StorageManager.MundaneBucket.CacheFromStorage();
            Console.WriteLine(" ... Mundane Templates Loaded: {0}", GlobalMundaneTemplateCache.Count);
        }

        public static void LoadWarpTemplates()
        {
            Console.WriteLine("\n----- Loading Warp Portals -----");
            StorageManager.WarpBucket.CacheFromStorage();
            Console.WriteLine(" ... Warp Templates Loaded: {0}", GlobalWarpTemplateCache.Count);
        }

        public static void LoadMaps()
        {
            Console.WriteLine("\n----- Loading Maps -----");
            StorageManager.AreaBucket.CacheFromStorage();
            Console.WriteLine(" -> Map Templates Loaded: {0}", GlobalMapCache.Count);
        }

        private static void StartServers()
        {
            Running = false;

            redo:
            if (ERRORS > Config.ERRORCAP)
                Process.GetCurrentProcess().Kill();

            try
            {
                Lobby = new LoginServer(Config.ConnectionCapacity);
                Lobby.Start(Config.LOGIN_PORT);
                Game = new GameServer(Config.ConnectionCapacity);
                Game.Start(DEFAULT_PORT);

                Running = true;
            }
            catch (Exception)
            {
                ++DEFAULT_PORT;
                ERRORS++;
                goto redo;
            }
        }

        /// <summary>
        ///     EP
        /// </summary>
        public virtual void Start()
        {
            Startup();
        }

        public static void Startup()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Config.SERVER_TITLE);
            Console.WriteLine("----------------------------------------------------------------------");

            LoadConstants();
            LoadAndCacheStorage();
            StartServers();

            Console.WriteLine("\n----------------------------------------------------------------------");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("{0} Online.", Config.SERVER_TITLE);
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static void EmptyCacheCollectors()
        {
            GlobalItemTemplateCache = new Dictionary<string, ItemTemplate>();
            GlobalMapCache = new Dictionary<int, Area>();
            GlobalMetaCache = new List<Metafile>();
            GlobalMonsterTemplateCache = new Dictionary<string, MonsterTemplate>();
            GlobalMundaneTemplateCache = new Dictionary<string, MundaneTemplate>();
            GlobalRedirects = new List<Redirect>();
            GlobalSkillTemplateCache = new Dictionary<string, SkillTemplate>();
            GlobalSpellTemplateCache = new Dictionary<string, SpellTemplate>();
            GlobalWarpTemplateCache = new Dictionary<int, List<WarpTemplate>>();
        }

        public static void LoadObjectCache()
        {
            var _cache_ = StorageManager.Load<ObjectService>();

            if (_cache_ != null)
                ObjectService.Set(_cache_);
        }

        public static void LoadConstants()
        {
            var _config_ = StorageManager.Load<ServerConstants>();

            if (_config_ == null)
            {
                Console.WriteLine("No config found. Generating defaults.");
                Config = new ServerConstants();
                StorageManager.Save(Config);
            }
            else
            {
                Config = StorageManager.Load<ServerConstants>();
            }

            if (Config.CacheObjects)
                LoadObjectCache();

            InitFromConfig();
        }

        public static void InitFromConfig()
        {
            DEFAULT_PORT = Config.SERVER_PORT;

            if (!Directory.Exists(STORAGE_PATH))
                Directory.CreateDirectory(STORAGE_PATH);
        }

        public static void LoadMetaDatabase()
        {
            Console.WriteLine("\n----- Loading Meta Database -----");
            GlobalMetaCache.AddRange(MetafileManager.GetMetafiles());
            Console.WriteLine(" -> Building Meta Cache: {0} loaded.", GlobalMetaCache.Count);
        }

        public static void LoadAndCacheStorage()
        {
            EmptyCacheCollectors();
            {
                LoadMetaDatabase();
                LoadMaps();
                LoadSkillTemplates();
                LoadSpellTemplates();
                LoadItemTemplates();
                LoadMonsterTemplates();
                LoadMundaneTemplates();
                LoadWarpTemplates();
            }
            Console.WriteLine("\n");



            GlobalItemTemplateCache["Boots"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 76,
                Image = 1,
                EquipmentSlot = ItemSlots.Foot,
                Gender = Gender.Both,
                DropRate = 0.04,
                CanStack = false,
                Value = 1000,
                LevelRequired = 1,
                MaxDurability = 3000,
                Weight = 2,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Boot",
                Class = Class.Peasant,
                Name = "Boots",
            };           

            GlobalItemTemplateCache["Ancuisa Ceir"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 331,
                Image = 331,
                EquipmentSlot = ItemSlots.Waist,
                Gender = Gender.Both,
                DropRate = 0.08,
                CanStack = false,
                Value = 2500,
                LevelRequired = 11,
                MaxDurability = 500,
                Weight = 1,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable | ItemFlags.Elemental,
                DefenseElement = ElementManager.Element.Fire,
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Belt",
                Class = Class.Peasant,
                Name = "Ancuisa Ceir",
            };

            GlobalItemTemplateCache["Silver Earrings"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 230,
                Image = 230,
                EquipmentSlot = ItemSlots.Earring,
                Gender = Gender.Both,
                DropRate = 0.10,
                CanStack = false,
                Value = 3000,
                LevelRequired = 8,
                MaxDurability = 1000,
                Weight = 1,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                StrModifer = new StatusOperator(StatusOperator.Operator.Add, 1),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Earring",
                Class = Class.Peasant,
                Name = "Silver Earrings"
            };

            GlobalItemTemplateCache["Gold Earrings"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 231,
                Image = 231,
                EquipmentSlot = ItemSlots.Earring,
                Gender = Gender.Both,
                DropRate = 0.10,
                CanStack = false,
                Value = 10000,
                LevelRequired = 8,
                MaxDurability = 2000,
                Weight = 1,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                StrModifer = new StatusOperator(StatusOperator.Operator.Add, 2),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Earring",
                Class = Class.Peasant,
                Name = "Gold Earrings"
            };


            GlobalItemTemplateCache["Leather Gauntlet"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 227,
                Image = 227,
                EquipmentSlot = ItemSlots.RArm,
                Gender = Gender.Both,
                DropRate = 0.02,
                CanStack = false,
                Value = 2000,
                LevelRequired = 9,
                MaxDurability = 3000,
                Weight = 2,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                AcModifer = new StatusOperator(StatusOperator.Operator.Remove, 1),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Generic",
                Class = Class.Peasant,
                Name = "Leather Gauntlet"
            };

            GlobalItemTemplateCache["Leather Greaves"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 238,
                Image = 238,
                EquipmentSlot = ItemSlots.Leg,
                Gender = Gender.Both,
                DropRate = 0.04,
                CanStack = false,
                Value = 4000,
                LevelRequired = 6,
                MaxDurability = 3000,
                Weight = 2,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                AcModifer = new StatusOperator(StatusOperator.Operator.Remove, 2),
                ManaModifer = new StatusOperator(StatusOperator.Operator.Remove, 500),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Generic",
                Class = Class.Peasant,
                Name = "Leather Greaves",
            };

            GlobalItemTemplateCache["Orc Helmet"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 533,
                Image = 2,
                EquipmentSlot = ItemSlots.Helmet,
                Gender = Gender.Both,
                DropRate = 0.01,
                CanStack = false,
                Value = 15000,
                LevelRequired = 96,
                MaxDurability = 5000,
                Weight = 3,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                AcModifer = new StatusOperator(StatusOperator.Operator.Remove, 2),
                ManaModifer = new StatusOperator(StatusOperator.Operator.Remove, 500),
                StrModifer = new StatusOperator(StatusOperator.Operator.Add, 2),
                IntModifer = new StatusOperator(StatusOperator.Operator.Remove, 2),
                WisModifer = new StatusOperator(StatusOperator.Operator.Remove, 2),
                DexModifer = new StatusOperator(StatusOperator.Operator.Remove, 2),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Helmet",
                Class = Class.Peasant,
                Name = "Orc Helmet",
            };

            GlobalItemTemplateCache["Loures Signet Ring"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 207,
                Image = 207,
                EquipmentSlot = ItemSlots.LHand,
                Gender = Gender.Both,
                DropRate = 0.01,
                CanStack = false,
                Value = 2500000,
                LevelRequired = 1,
                MaxDurability = 2000,
                Weight = 1,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                AcModifer = new StatusOperator(StatusOperator.Operator.Remove, 1),                
                ManaModifer = new StatusOperator(StatusOperator.Operator.Add, 100),
                HealthModifer = new StatusOperator(StatusOperator.Operator.Add, 100),
                StrModifer = new StatusOperator(StatusOperator.Operator.Add, 1),
                IntModifer = new StatusOperator(StatusOperator.Operator.Add, 1),
                WisModifer = new StatusOperator(StatusOperator.Operator.Add, 1),
                DexModifer = new StatusOperator(StatusOperator.Operator.Add, 1),
                ConModifer = new StatusOperator(StatusOperator.Operator.Add, 1),
                MrModifer = new StatusOperator(StatusOperator.Operator.Add, 10),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Generic",
                Class = Class.Peasant,
                Name = "Loures Signet Ring"
            };

            GlobalItemTemplateCache["Eternal Love Ring"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 1101,
                Image = 1101,
                EquipmentSlot = ItemSlots.LHand,
                Gender = Gender.Both,
                DropRate = 0.01,
                CanStack = false,
                Value = 1000000,
                LevelRequired = 21,
                MaxDurability = 1000000,
                Weight = 1,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                MrModifer = new StatusOperator(StatusOperator.Operator.Add, 20),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Generic",
                Class = Class.Peasant,
                Name = "Eternal Love Ring"
            };

            GlobalItemTemplateCache["Black Stone Ring"] = new ItemTemplate()
            {
                DisplayImage = 0x8000 + 1360,
                Image = 1360,
                EquipmentSlot = ItemSlots.LHand,
                Gender = Gender.Both,
                DropRate = 0.01,
                CanStack = false,
                Value = 50000,
                LevelRequired = 99,
                StageRequired = ClassStage.Master,
                MaxDurability = 5000,
                Weight = 1,
                Flags = ItemFlags.Bankable | ItemFlags.Dropable | ItemFlags.Equipable | ItemFlags.Repairable | ItemFlags.Tradeable | ItemFlags.Upgradeable,
                MrModifer = new StatusOperator(StatusOperator.Operator.Add, 10),
                AcModifer = new StatusOperator(StatusOperator.Operator.Remove, 3),
                HealthModifer = new StatusOperator(StatusOperator.Operator.Add, 400),
                ManaModifer = new StatusOperator(StatusOperator.Operator.Add, 50),
                StrModifer = new StatusOperator(StatusOperator.Operator.Remove, 1),
                Upgrades = 0,
                NpcKey = "shop1",
                ScriptName = "Generic",
                Class = Class.Peasant,
                Name = "Black Stone Ring"
            };

        }
    }
}