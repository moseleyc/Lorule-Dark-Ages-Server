using System;

namespace Darkages.Network
{
    public class NetworkPacket
    {
        public byte Command { get; set; }
        public byte Ordinal { get; set; }
        public byte[] Data { get; set; }

        public NetworkPacket(byte[] array, int start, int count)
        {
            this.Command = array[start + 0];
            this.Ordinal = array[start + 1];
            this.Data = new byte[count - 2];

            if (this.Data.Length != 0)
            {
                Array.Copy(array, start + 2, this.Data, 0, this.Data.Length);
            }
        }

        public byte[] ToArray()
        {
            var buffer = new byte[this.Data.Length + 5];

            buffer[0] = 0xAA;
            buffer[1] = (byte)((this.Data.Length + 2) >> 8);
            buffer[2] = (byte)((this.Data.Length + 2) >> 0);
            buffer[3] = this.Command;
            buffer[4] = this.Ordinal;

            for (int i = 0; i < this.Data.Length; i++)
            {
                buffer[i + 5] = this.Data[i];
            }

            return buffer;
        }

        public override string ToString()
        {
            return string.Format("{0:X2} {1:X2} {2}",
                this.Command,
                this.Ordinal,
                BitConverter.ToString(this.Data).Replace('-', ' '));
        }
    }
}