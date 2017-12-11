namespace Darkages.Network.ClientFormats
{
    public class ClientFormat7B : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x7B; }
        }

        public byte Type { get; set; }

        #region Type 0 Variables

        public string Name { get; set; }

        #endregion

        #region Type 1 Variables

        #endregion

        public override void Serialize(NetworkPacketReader reader)
        {
            this.Type = reader.ReadByte();

            #region Type 0

            if (this.Type == 0x00)
            {
                this.Name = reader.ReadStringA();
            }

            #endregion

            #region Type 1

            if (this.Type == 0x01)
            {
            }

            #endregion
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
        }
    }
}