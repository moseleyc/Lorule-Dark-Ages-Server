using System;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat39 : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x39; }
        }

        public Aisling Aisling { get; set; }

        public ServerFormat39(Aisling aisling)
        {
            this.Aisling = aisling;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter packet)
        {
            packet.Write((byte)Aisling.Nation);
            packet.WriteStringA(Aisling.ClanRank);

            packet.Write((byte)0x07);
            packet.Write((byte)0x0);
            packet.Write((byte)0x0);
            packet.Write((byte)0x0);
            packet.Write((byte)0x0);
            packet.Write((byte)0x0);
            packet.Write((byte)0x0);
            packet.Write((byte)0x0);

            packet.WriteStringA("No Group");


            packet.Write((byte)Aisling.PartyStatus);
            packet.Write((byte)0x00);
            packet.Write((byte)Aisling.ClassID);
            packet.Write((byte)Aisling.Nation);
            packet.Write((byte)0x00);
            packet.WriteStringA(Convert.ToString((Aisling.Stage
                == Types.ClassStage.Master) ? "Master" : Aisling.Path.ToString()));
            packet.WriteStringA(Aisling.ClanTitle);

            packet.Write((byte)Aisling.LegendBook.LegendMarks.Count);
            foreach (var legend in Aisling.LegendBook.LegendMarks)
            {
                packet.Write((byte)legend.Icon);
                packet.Write((byte)legend.Color);
                packet.WriteStringA(legend.Category);
                packet.WriteStringA(legend.Value);
            }

            packet.Write((byte)0x00);
            packet.Write((ushort)Aisling.Display);
            packet.Write((byte)0x02);
            packet.Write((uint)0x00);
            packet.Write((byte)0x00);
        }
    }
}
