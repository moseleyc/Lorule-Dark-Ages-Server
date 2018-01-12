using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Darkages.Types;
using Darkages.Common;
using static Darkages.Common.Extensions;

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
                        if (template != null)
                        {
                            if (template.SpawnOnlyOnActiveMaps && !map.Has<Aisling>())
                                continue;

                            using (new DisposableStopwatch(t => Console.WriteLine("{0} elapsed", t)))
                            {
                                Task.Run(() => SpawnOn(template, map));
                            }
                        }
                }
            }
        }

        public bool SpawnOn(MonsterTemplate template, Area map)
        {
            var count = GetObjects<Monster>(i => i.Template.Name == template.Name).Length;

            if (count < template.SpawnMax)
            {
                count = GetObjects<Monster>(i => i.Template.Name == template.Name).Length;

                if ((template.SpawnType & SpawnQualifer.Random) == SpawnQualifer.Random)
                {
                    var needed = Math.Abs(count - template.SpawnSize);
                    for (var i = needed - 1; i >= 0; i--)
                    {
                        CreateFromTemplate<Monster>(template, map);
                    }
                }
                else if ((template.SpawnType & SpawnQualifer.Defined) == SpawnQualifer.Defined)
                    CreateFromTemplate<Monster>(template, map);
            }

            return false;
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