using Darkages.Types;
using System.Text;

namespace Darkages.Network
{
    public class NetworkPacketReader
    {
        private Encoding encoding = Encoding.GetEncoding(949);

        public NetworkPacket Packet { get; set; }
        public int Position { get; set; }
        public bool CanRead => Position + 1 < Packet.Data.Length;

        public T ReadObject<T>()
            where T : IFormattable, new()
        {
            T result = new T();

            result.Serialize(this);

            return result;
        }

        public bool ReadBool()
        {
            return (this.ReadByte() != 0);
        }

        public Position ReadPosition()
        {
            var pos = new Position()
            {
                X = this.ReadUInt16(),
                Y = this.ReadUInt16()
            };
            return pos;
        }

        public byte ReadByte()
        {
            byte b;

            if (Position == -1)
            {
                b = Packet.Ordinal;
            }
            else
            {
                b = Packet.Data[Position];
            }

            Position++;

            return b;
        }
        public byte[] ReadBytes(int count)
        {
            byte[] array = new byte[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = ReadByte();
            }

            return array;
        }

        public string ReadStringA()
        {
            var length = this.ReadByte();
            var result = this.encoding.GetString(this.Packet.Data, this.Position, length);

            this.Position += length;

            return result;
        }
        public string ReadStringB()
        {
            var length = this.ReadUInt16();
            var result = this.encoding.GetString(this.Packet.Data, this.Position, length);

            this.Position += length;

            return result;
        }

        public short ReadInt16()
        {
            return (short)this.ReadUInt16();
        }
        public ushort ReadUInt16()
        {
            return (ushort)((
                this.ReadByte() << 8) |
                this.ReadByte());
        }

        public int ReadInt32()
        {
            return (int)this.ReadUInt32();
        }
        public uint ReadUInt32()
        {
            return (uint)((
                this.ReadUInt16() << 0x10) +
                this.ReadUInt16());
        }
    }
}