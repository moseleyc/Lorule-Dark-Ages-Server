using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Darkages.Types;
using Darkages.Common;
using static Darkages.Common.Extensions;
using System.Threading;
using System.Collections.Generic;

namespace Darkages.Network.Game.Components
{
    public class MonolithComponent : GameServerComponent
    {
        private readonly GameServerTimer _timer;
        private readonly Thread _spawnThread;

        public MonolithComponent(GameServer server)
            : base(server)
        {
            _timer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.GlobalSpawnTimer));
            _spawnThread = new Thread(new ThreadStart(SpawnConsumer));
            _spawnThread.IsBackground = true;
            _spawnThread.Start();
        }

        public Queue<Spawn> SpawnQueue = new Queue<Spawn>();
        public object SyncObj = new object();

        public class Spawn
        {
            public DateTime LastSpawned { get; set; }
            public MonsterTemplate Template { get; set; }
            public Area Map { get; set; }
        }

        private void SpawnConsumer()
        {
            while (true)
            {

                Spawn spawnObj = null;

                lock (SyncObj) {

                    if (SpawnQueue.Count > 0)
                    {
                        spawnObj = SpawnQueue.Dequeue();
                    }
                }

                if (spawnObj != null)
                {
                    SpawnOn(spawnObj.Template, spawnObj.Map);
                }

                Thread.Sleep(100);
            }
        }

        public override void Update(TimeSpan elapsedTime)
        {
            _timer.Update(elapsedTime);

            if (_timer.Elapsed)
            {
                _timer.Reset();

                var templates = ServerContext.GlobalMonsterTemplateCache.Values;
                if (templates.Count == 0)
                    return;

                foreach (var map in ServerContext.GlobalMapCache.Values)
                {
                    if (map == null || map.Rows == 0 || map.Cols == 0)
                        return;

                    var temps = templates.Where(i => i.AreaID == map.ID);
                    foreach (var template in temps)
                    {
                        if (template != null)
                        {
                            if (template.SpawnOnlyOnActiveMaps && !map.Has<Aisling>())
                                continue;

                            var spawn = new Spawn()
                            {
                                Template = template,
                                Map = map
                            };

                            lock (SyncObj)
                            {
                                SpawnQueue.Enqueue(spawn);
                            }
                        }
                    }
                }
            }
        }

        public void SpawnOn(MonsterTemplate template, Area map)
        {
            var count = GetObjects<Monster>(i => i.Template.Name == template.Name).Length;

            if (count < template.SpawnMax)
            {
                if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
                {
                    CreateFromTemplate<Monster>(template, map);
                }
                else if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
                    CreateFromTemplate<Monster>(template, map);
            }
        }


        public bool CreateFromTemplate<T>(Template template, Area map) where T : Sprite, new()
        {
            var obj = new T();

            if (obj is Monster)
            {
                var newObj = Monster.Create(template as MonsterTemplate, map);

                if (GetObject<Monster>(i => i.Serial == newObj.Serial) == null)
                {
                    AddObject(newObj);
                    return true;
                }
            }
            return false;
        }


    }
}