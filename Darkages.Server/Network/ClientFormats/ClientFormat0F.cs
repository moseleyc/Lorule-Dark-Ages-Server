using Darkages.Types;
using System;

namespace Darkages.Network.ClientFormats
{
    public class ClientFormat0F : NetworkFormat
    {
        public override bool Secured => true;

        public override byte Command => 0x0F;

        public byte Index { get; set; }
        public Position Point { get; set; }
        public uint Serial { get; set; }
        public string Data { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
            Index = reader.ReadByte();

            var @data = "";
            var @char = (default(char));

            do
            {
                @char = (char)Convert.ToChar(reader.ReadByte());
                data += new string(@char, 1);
            }
            while (@char != Char.Parse("\0"));

            reader.Position = 2;
            if (reader.CanRead)
                Serial = reader.ReadUInt32();

            if (reader.CanRead)
                Point = reader.ReadPosition();

            Data = data.Trim('\0');
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}