using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Types;
using Newtonsoft.Json;

namespace Darkages
{
    public class PortalSession
    {
        public PortalSession()
        {
            IsMapOpen = false;
        }

        public bool IsMapOpen { get; set; }
        public int FieldNumber { get; set; }

        [JsonIgnore]
        public WorldMapTemplate Template
            => ServerContext.GlobalWorldMapTemplateCache[FieldNumber];

        public void ShowFieldMap(GameClient client)
        {
            if (client.Aisling.PortalSession == null)
                client.Aisling.PortalSession
                    = new PortalSession
                    {
                        FieldNumber = 1,
                        IsMapOpen = false
                    };

            client.Send(new ServerFormat2E(client.Aisling));
            IsMapOpen = true;
        }

        public void TransitionToMap(GameClient client,
            short X = -1, short Y = -1, int DestinationMap = 0)
        {
            if (DestinationMap == 0)
            {
                client.LeaveArea(true, true);

                DestinationMap = ServerContext.Config.TransitionZone;
                var targetMap = ServerContext.GlobalMapCache[DestinationMap];
                client.Aisling.Map = targetMap;
                client.Aisling.X = X >= 0 ? X : ServerContext.Config.TransitionPointX;
                client.Aisling.Y = Y >= 0 ? Y : ServerContext.Config.TransitionPointY;
                client.Aisling.AreaID = DestinationMap;
                client.Aisling.CurrentMapId = DestinationMap;
                client.Refresh();
                ShowFieldMap(client);
                return;
            }

            if (ServerContext.GlobalMapCache.ContainsKey(DestinationMap))
            {
                var targetMap = ServerContext.GlobalMapCache[DestinationMap];

                if (client.Aisling.AreaID != DestinationMap)
                {
                    client.LeaveArea(true, false);
                    client.Aisling.Map = targetMap;
                    client.Aisling.X = X >= 0 ? X : ServerContext.Config.TransitionPointX;
                    client.Aisling.Y = Y >= 0 ? Y : ServerContext.Config.TransitionPointY;
                    client.Aisling.AreaID = DestinationMap;
                    client.Aisling.CurrentMapId = DestinationMap;
                    client.EnterArea();
                    client.Refresh();
                }
            }
        }
    }
}