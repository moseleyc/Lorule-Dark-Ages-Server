using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat30 : NetworkFormat
    {
        public override bool Secured => true;
        public override byte Command => 0x30;

        private Step Step { get; set; }

        public ServerFormat30(Step step)
        {
            this.Step = step;
        }

        public override void Serialize(NetworkPacketReader reader)
        {

        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)0x00); // type!
            writer.Write((byte)0x01); // ??
            writer.Write((uint)Step.Serial);
            writer.Write((byte)0x00); // ??
            writer.Write((ushort)Step.Image);
            writer.Write((byte)0x00); // ??
            writer.Write((byte)0x01); // ??
            writer.Write((ushort)Step.ScriptId);
            writer.Write((byte)0x00);
            writer.Write(ushort.MinValue);
            writer.Write((byte)Step.StepId);
            writer.Write((byte)Step.StepId);
            writer.Write((byte)Step.StepId);
            writer.Write((bool)Step.HasNext); 
            writer.Write((bool)Step.HasBack); 
            writer.WriteStringA(Step.Title);
            writer.WriteStringB(Step.Body);
        }
    }
}
