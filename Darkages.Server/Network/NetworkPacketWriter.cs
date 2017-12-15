using Darkages.Network.Game;
using System;
using System.Net;
using System.Text;

namespace Darkages.Network
{
    public class NetworkPacketWriter
    {
        private Encoding encoding = Encoding.GetEncoding(949);
        private byte[] buffer = new byte[ServerContext.Config.BufferSize];

        public int Position { get; set; }
        public bool CanWrite => this.Position + 1 < this.buffer.Length;

        public void Write(bool value)
        {
            this.Write(
                (byte)(value ? 1 : 0));
        }
        public void Write(byte value)
        {
            this.buffer[this.Position++] = value;
        }
        public void Write(byte[] value)
        {
            Array.Copy(value, 0, this.buffer, this.Position, value.Length);
            this.Position += value.Length;
        }

        public void Write(short value)
        {
            this.Write(
                (ushort)value);
        }
        public void Write(ushort value)
        {
            this.Write(
                (byte)(value >> 8));
            this.Write(
                (byte)(value));
        }
        public void Write(int value)
        {
            this.Write(
                (uint)value);
        }
        public void Write(uint value)
        {
            this.Write(
                (ushort)(value >> 16));
            this.Write(
                (ushort)(value));
        }

        public void Write<T>(T value)
            where T : IFormattable
        {
            value.Serialize(this);
        }
        public void WriteString(string value)
        {
            this.encoding.GetBytes(value, 0, value.Length, this.buffer, this.Position);
            this.Position += this.encoding.GetByteCount(value);
        }
        public void WriteStringA(string value)
        {
            var count = this.encoding.GetByteCount(value);

            this.Write(
                (byte)count);

            this.encoding.GetBytes(value, 0, value.Length, this.buffer, this.Position);
            this.Position += count;
        }
        public void WriteStringB(string value)
        {
            var count = this.encoding.GetByteCount(value);

            this.Write(
                (ushort)count);

            this.encoding.GetBytes(value, 0, value.Length, this.buffer, this.Position);
            this.Position += count;
        }

        public void Write(IPEndPoint endPoint)
        {
            byte[] ipBytes = endPoint.Address.GetAddressBytes();

            this.Write(ipBytes[3]);
            this.Write(ipBytes[2]);
            this.Write(ipBytes[1]);
            this.Write(ipBytes[0]);
            this.Write(
                (ushort)endPoint.Port);
        }

        public NetworkPacket ToPacket()
        {
            return new NetworkPacket(this.buffer, 0, this.Position);
        }
    }
}