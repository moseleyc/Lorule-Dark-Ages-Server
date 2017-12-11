using Darkages.IO;

namespace Darkages.Network.ServerFormats
{


    public class ServerFormat15 : NetworkFormat
    {
        public override bool Secured
        {
            get
            {
                return true;
            }
        }
        public override byte Command
        {
            get
            {
                return 0x15;
            }
        }

        public Area Area { get; set; }

        public ServerFormat15(Area area)
        {
            Area = area;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((ushort)Area.Number);
            writer.Write((byte)Area.Cols); // W
            writer.Write((byte)Area.Rows); // H
            writer.Write((byte)Area.Flags);
            writer.Write((byte)0x00);
            writer.Write((byte)0x00);
            writer.Write((ushort)Area.Hash);
            writer.WriteStringA(Area.Name);

        }
    }
}
