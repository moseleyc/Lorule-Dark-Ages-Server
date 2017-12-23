using Darkages.Network.Game;
using Darkages.Network.Login;
using Darkages.Network.Object;
using Darkages.Storage;
using Darkages.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Darkages
{
    public class ServerContext : ObjectManager
    {
        Lazy<ServerContext> _instance = new Lazy<ServerContext>(true);

        public static ServerConstants Config = null;

        public static int ERRORS = 0;
        public static int DEFAULT_PORT;
        public static IPAddress IPADDRESS => IPAddress.Parse(File.ReadAllText(Config.ServerTablePath));
        public static string STORAGE_PATH => @"..\..\..\Storage\Locales";


        public static List<Redirect> GlobalRedirects = new List<Redirect>();
        public static List<Metafile> GlobalMetaCache = new List<Metafile>();
        public static Dictionary<int, Area> GlobalMapCache = new Dictionary<int, Area>();
        public static Dictionary<string, MonsterTemplate> GlobalMonsterTemplateCache = new Dictionary<string, MonsterTemplate>();
        public static Dictionary<string, SkillTemplate> GlobalSkillTemplateCache = new Dictionary<string, SkillTemplate>();
        public static Dictionary<string, SpellTemplate> GlobalSpellTemplateCache = new Dictionary<string, SpellTemplate>();
        public static Dictionary<string, ItemTemplate> GlobalItemTemplateCache = new Dictionary<string, ItemTemplate>();
        public static Dictionary<string, MundaneTemplate> GlobalMundaneTemplateCache = new Dictionary<string, MundaneTemplate>();
        public static Dictionary<int, List<WarpTemplate>> GlobalWarpTemplateCache = new Dictionary<int, List<WarpTemplate>>();

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
                System.Diagnostics.Process.GetCurrentProcess().Kill();

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
                ++DEFAULT_PORT; ERRORS++;
                goto redo;
            }
        }

        /// <summary>
        /// EP
        /// </summary>
        public virtual void Start() => Startup();

        public static bool Running = false;

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

        public static void LoadObjectCache()
        {
            var _cache_ = StorageManager.Load<ObjectService>();

            if (_cache_ != null)
            {
                ObjectService.Set(_cache_);
            }
        }

        public static void LoadConstants()
        {
            var _config_ = StorageManager.Load<ServerConstants>();
            
            if (_config_ == null)
            {
                Console.WriteLine("No config found. Generating defaults.");
                Config = new ServerConstants();
                StorageManager.Save<ServerConstants>(Config);
            }
            else
            {
                Config = StorageManager.Load<ServerConstants>();
            }

            if (Config.CacheObjects)
            {
                LoadObjectCache();
            }

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
            LoadMetaDatabase();
            LoadMaps();
            LoadSkillTemplates();
            LoadSpellTemplates();
            LoadItemTemplates();
            LoadMonsterTemplates();
            LoadMundaneTemplates();
            LoadWarpTemplates();
            Console.WriteLine("\n");
        }
    }
}
