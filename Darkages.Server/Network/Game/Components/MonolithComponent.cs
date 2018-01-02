using System;
using System.Linq;
using System.Threading.Tasks;
using Darkages.Types;

namespace Darkages.Network.Game.Components
{
    public class MonolithComponent : GameServerComponent
    {
        private readonly GameServerTimer _timer;

        public MonolithComponent(GameServer server)
            : base(server)
        {
            _timer = new GameServerTimer(TimeSpan.FromMilliseconds(ServerContext.Config.GlobalSpawnTimer));
        }

        public DateTime LastUpdate { get; set; }

        public override void Update(TimeSpan elapsedTime)
        {
            _timer.Update(elapsedTime);

            if (_timer.Elapsed)
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
                        if (template != null)
                        {
                            if (template.Timer == null)
                                template.Timer = new GameServerTimer(TimeSpan.FromSeconds(template.SpawnRate));

                            template.Timer.Update(DateTime.UtcNow - template.LastUpdate);
                            if (template.Timer.Elapsed)
                            {
                                if (template.SpawnOnlyOnActiveMaps && !map.Has<Aisling>())
                                    continue;

                                SpawnOn(template, map);
                                template.LastUpdate = DateTime.UtcNow;
                                template.Timer.Reset();
                            }
                        }
                }

                _timer.Reset();
            }
        }

        public void SpawnOn(MonsterTemplate template, Area map)
        {
            new TaskFactory().StartNew(() =>
            {
                var count = GetObjects<Monster>(i => i.Template.Name == template.Name).Length;

                if (count < template.SpawnMax)
                {
                    if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
                    {
                        var needed = Math.Abs(count - template.SpawnSize);
                        for (var i = needed - 1; i >= 0; i--)
                            CreateFromTemplate<Monster>(template, map);
                    }
                    if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
                        CreateFromTemplate<Monster>(template, map);
                }
            });
        }

        public void CreateFromTemplate<T>(Template template, Area map) where T : Sprite, new()
        {
            var obj = new T();

            if (obj is Monster)
            {
                var newObj = Monster.Create(template as MonsterTemplate, map);

                if (GetObject<Monster>(i => i.Serial == newObj.Serial) == null)
                    AddObject(newObj);
            }
        }
    }
}