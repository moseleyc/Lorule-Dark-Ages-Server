using Darkages.Network.Game;
using Darkages.Scripting;
using Darkages.Types;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Storage.locales.Scripts.Mundanes
{
    [Script("shop1", "Dean")]
    public class shop1 : MundaneScript
    {
        public shop1(GameServer server, Mundane mundane) : base(server, mundane)
        {

        }

        public override void OnClick(GameServer server, GameClient client)
        {
            client.SendItemShopDialog(Mundane, "What you looking for?", 1, ServerContext.GlobalItemTemplateCache.Values );
        }

        public override void OnResponse(GameServer server, GameClient client, short responseID, string args)
        {
            if (string.IsNullOrEmpty(args))
                return;

            if (!ServerContext.GlobalItemTemplateCache.ContainsKey(args))
                return;

            var template = ServerContext.GlobalItemTemplateCache[args];
            if (template != null)
            {
                //Create Item:
                var item = Item.Create(client.Aisling, template);
                item.GiveTo(client.Aisling);
            }
        }
    }
}
