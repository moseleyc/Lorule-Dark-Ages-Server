
using Darkages.Types;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;

namespace Darkages.Network.Game.Components
{
    public class MonolithComponent : GameServerComponent
    {
        private readonly GameServerTimer _timer;

        public readonly BlockingCollection<Spawn> SpawnQueue = new BlockingCollection<Spawn>();

        public object SyncObj = new object();

        public MonolithComponent(GameServer server)
            : base(server)
        {
            _timer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.GlobalSpawnTimer));
            var spawnThread = new Thread(SpawnEmitter) { IsBackground = true };
            spawnThread.Start();
        }

        private void SpawnEmitter()
        {
            while (true)
            {
                if (SpawnQueue.Count > 0)
                    ConsumeSpawns();

                Thread.Sleep(1000);
            }
        }

        private void ConsumeSpawns()
        {
            foreach (var spawnObj in SpawnQueue.GetConsumingEnumerable())
            {
                SpawnOn(spawnObj.Template, spawnObj.Map);
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
                        if (template.SpawnOnlyOnActiveMaps && !map.Has<Aisling>())
                            continue;

                        if (template.ReadyToSpawn())
                        {
                            var spawn = new Spawn
                            {
                                Template = template,
                                Map = map
                            };

                            if (!SpawnQueue.TryAdd(spawn))
                            {

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
                if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
                    CreateFromTemplate(template, map, count);
                else if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
                    CreateFromTemplate(template, map, count);
        }


        public bool CreateFromTemplate(MonsterTemplate template, Area map, int count)
        {
            var newObj = Monster.Create(template as MonsterTemplate, map);
            AddObject(newObj);

            return false;
        }

        public class Spawn
        {
            public DateTime LastSpawned { get; set; }
            public MonsterTemplate Template { get; set; }
            public Area Map { get; set; }
        }
    }
}

