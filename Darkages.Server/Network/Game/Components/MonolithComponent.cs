using Darkages.Types;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Darkages.Network.Game.Components
{
    public class MonolithComponent : GameServerComponent
    {
        private GameServerTimer timer;
        private DateTime LastUpdate = DateTime.UtcNow;
        private GameServerTimer UpdateEventScheduler = null;



        public MonolithComponent(GameServer server)
            : base(server)
        {
            UpdateEventScheduler = new GameServerTimer(TimeSpan.FromSeconds(10));
            timer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.GlobalSpawnTimer));
        }

        public override void Update(TimeSpan elapsedTime)
        {
            timer.Update(elapsedTime);

            if (timer.Elapsed)
            {
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
                            if (template.Timer == null)
                                template.Timer = new GameServerTimer(TimeSpan.FromSeconds(template.SpawnRate));

                            template.Timer.Update(DateTime.UtcNow - LastUpdate);
                            if (template.Timer.Elapsed)
                            {
                                if (template.SpawnOnlyOnActiveMaps && !map.Has<Aisling>())
                                    continue;

                                SpawnOn(template, map);
                                template.Timer.Reset();
                                LastUpdate = DateTime.UtcNow;
                            }
                        }
                    }
                }

                timer.Reset();
            }
        }

        public void SpawnOn(MonsterTemplate template, Area map)
        {
            var count = GetObjects<Monster>(i => i.Template.Name == template.Name).Length;

            if (count < template.SpawnMax)
            {
                if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
                {
                    for (int i = 0; i < template.SpawnSize; i++)
                        CreateFromTemplate<Monster>(template, map);
                }
                if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
                {
                    CreateFromTemplate<Monster>(template, map);
                }
            }
        }

        public void CreateFromTemplate<T>(Template template, Area map) where T : Sprite, new()
        {
            var obj = new T();

            if (obj is Monster)
            {
                new TaskFactory().StartNew(() => {
                    var newObj = Monster.Create(template as MonsterTemplate, map);

                    if (GetObject<Monster>(i => i.Serial == newObj.Serial) == null)
                    {                        
                        AddObject<Monster>(newObj);
                    }
                });
            }
        }
    }
}
