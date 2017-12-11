using Darkages.Types;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat17 : NetworkFormat
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
                return 0x17;
            }
        }
        
        public Spell Spell { get; set; }
        
        public ServerFormat17(Spell spell)
        {
            this.Spell = spell;
        }

        public override void Serialize(NetworkPacketReader reader)
        {

        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)Spell.Slot);
            writer.Write((ushort)Spell.Template.Icon);
            writer.Write((byte)Spell.Template.TargetType);
            writer.WriteStringA(Spell.Name);
            writer.WriteStringA(Spell.Template.Text);
            writer.Write((byte)Spell.Lines);
        }
    }
}
