using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat31 : NetworkFormat
    {
        public ServerFormat31(Forum forum)
        {
            Forum = forum;
        }

        public override bool Secured => true;

        public override byte Command => 0x31;

        public Forum Forum { get; set; }

        public override void Serialize(NetworkPacketReader reader)
        {
        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            if (Forum is Board)
            {
                writer.Write((byte)0x01);
                writer.Write((ushort)Forum.Topics.Count);

                foreach (var topic in Forum.Topics)
                {
                    writer.Write((ushort)topic.Number);
                    writer.WriteStringA(topic.Title);
                }
            }
            else if (Forum is Topic)
            {
                writer.Write((byte)0x02);
                writer.Write((byte)0x01);
                writer.Write((ushort)Forum.Number);
                writer.WriteStringA((Forum as Topic).Title);           
                writer.Write((byte)(Forum as Topic).Posts.Count);

                foreach (var topic in (Forum as Topic).Posts)
                {
                    writer.Write((byte)(topic.Bold ? 1 : 0));
                    writer.Write((ushort)topic.TopicNumber);
                    writer.WriteStringA(topic.Author);
                    writer.Write((byte)topic.DatePosted.Month);
                    writer.Write((byte)topic.DatePosted.Day);
                    writer.WriteStringA(topic.Message);
                }
            }
        }
    }
}