using Darkages.Common;
using Darkages.Network;
using System.Text;

namespace Darkages.Security
{
    [System.Serializable]
    public sealed class SecurityParameters : IFormattable
    {
        public static readonly SecurityParameters Default = new SecurityParameters(0, Encoding.ASCII.GetBytes(ServerContext.Config?.DefaultKey ?? "NexonInc."));

        public byte Seed { get; private set; }
        public byte[] Salt { get; private set; }

        public SecurityParameters()
        {
            lock (Generator.Random)
            {
                this.Seed = (byte)Generator.Random.Next(0, 9);
                this.Salt = Generator.GenerateString(9).ToByteArray();
            }
        }
        public SecurityParameters(byte seed, byte[] key)
        {
            this.Seed = seed;
            this.Salt = key;
        }

        public void Serialize(NetworkPacketReader reader)
        {
            this.Seed = reader.ReadByte();
            this.Salt = reader.ReadBytes(reader.ReadByte());
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(this.Seed);
            writer.Write(
                (byte)this.Salt.Length);
            writer.Write(this.Salt);
        }
    }
}