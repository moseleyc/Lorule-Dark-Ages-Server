using Darkages.Network.Game;
using Darkages.Types;
using System;

namespace Darkages.Storage.locales.Buffs
{
    public class buff_dion : Buff
    {
        /// <summary>
        /// This name MUST match and correspond the name in the type BUFF.
        /// </summary>
        public override string Name
        {
            get => "dion";
        }

        public buff_dion(GameClient client) : base(client)
        {
            StatusBarItem = new StatusItem<Buff>(client, 6, 53, this.Name, (this as Buff));
        }

        public override void OnApplied(Buff buff, StatusItem<Buff> item)
        {
            if (Client == null || Client.Aisling == null)
                return;

            if (!Client.Aisling.LoggedIn)
                return;

            Client.Aisling.Status |= Status.dion;

            Client.SendMessage(0x02, "Your skin turns to stone.");
        }

        public override void OnDurationUpdate(Buff buff, StatusItem<Buff> item)
        {

        }

        public override void OnEnded(Buff buff, StatusItem<Buff> item)
        {
            Client.Aisling.Status ^= Status.dion;

            Client.SendMessage(0x02, "Your skin turns back to flesh.");
        }
    }
}