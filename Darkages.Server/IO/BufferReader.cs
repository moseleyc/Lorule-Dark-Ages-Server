using System.IO;
using System.Net;
using System.Text;

namespace Darkages.IO
{
    public class BufferReader : BinaryReader
    {
        private Encoding encoding = Encoding.GetEncoding(949);

        public BufferReader(Stream stream)
            : base(stream, Encoding.GetEncoding(949))
        {
        }

        public IPAddress ReadIPAddress()
        {
            byte[] ipBuffer = new byte[4];

            ipBuffer[3] = base.ReadByte();
            ipBuffer[2] = base.ReadByte();
            ipBuffer[1] = base.ReadByte();
            ipBuffer[0] = base.ReadByte();

            return new IPAddress(ipBuffer);
        }
        public string ReadStringA()
        {
            return encoding.GetString(
                base.ReadBytes(base.ReadByte()));
        }
        public string ReadStringB()
        {
            return encoding.GetString(
                base.ReadBytes(this.ReadUInt16()));
        }

        public override string ReadString()
        {
            var data = ' ';
            var text = "";

            do
            {
                text += (data = base.ReadChar());
            }
            while (data != '\0');

            return text;
        }

        public override short ReadInt16()
        {
            return (short)this.ReadUInt16();
        }
        public override ushort ReadUInt16()
        {
            return (ushort)((
                base.ReadByte() << 8) |
                base.ReadByte());
        }
        public override int ReadInt32()
        {
            return (int)this.ReadUInt32();
        }
        public override uint ReadUInt32()
        {
            return (uint)((
                this.ReadUInt16() << 16) |
                this.ReadUInt16());
        }
    }
}