using System.Collections.Generic;
using Darkages.Network.Game;
using Darkages.Network.ServerFormats;
using Darkages.Scripting;
using Darkages.Types;
using System.Linq;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("User Helper Menu")]
    public class UserHelper : MundaneScript
    {
        public UserHelper(GameServer server, Mundane mundane) : base(server, mundane)
        { 

        }

        public override void OnClick(GameServer server, GameClient client)
        {
            client.SendOptionsDialog(Mundane, "What do you need?",
                new OptionsDataItem(0x0001, "Warp"));
        }

        public override void OnResponse(GameServer server, GameClient client, ushort responseID, string args)
        {
            switch (responseID)
            {
                case 0x0001:
                {
                    var opts = new List<OptionsDataItem>();

                    var map = ServerContext.GlobalWarpTemplateCache;
                    var warp_names = map.Values.SelectMany(i => i).Where(i => i != null && i.WarpType == WarpType.Map && i.To != null && i.From != null);

                    foreach (var warp in warp_names)
                    {
                        opts.Add(new OptionsDataItem(
                            (short) warp.To.AreaID,  warp.Name));
                    }

                    client.SendOptionsDialog(Mundane, "You may travel to these location.",
                        opts.ToArray()
                    );
                }
                    break;
                default:

                    if (responseID > 0)
                    {
                        var key = responseID;

                        if (ServerContext.GlobalMapCache.ContainsKey(key))
                        {
                            //try and find a valid warp.
                            if (ServerContext.GlobalWarpTemplateCache.ContainsKey(key))
                            {
                                var warp = ServerContext.GlobalWarpTemplateCache.Values
                                    .FirstOrDefault(i => i.Count(n => n.To != null && n.To.AreaID == key) > 0)
                                    .FirstOrDefault() ?? ServerContext.GlobalWarpTemplateCache.Values
                                    .FirstOrDefault(i => i.Count(n => n.From != null && n.From.AreaID == key) > 0)
                                    .FirstOrDefault();

                                if (warp != null)
                                {
                                    client.WarpTo(warp);
                                }
                                else
                                {
                                    client.SendMessage(0x02, "You can't travel there at this time.");
                                    return;
                                }

                            }
                        }
                    }

                    break;
            }
        }

        public override void OnGossip(GameServer server, GameClient client, string message)
        {

        }

        public override void TargetAcquired(Sprite Target)
        {

        }
    }
}