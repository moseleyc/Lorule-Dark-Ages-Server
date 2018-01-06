namespace Darkages.Network.ServerFormats
{
    public class ServerFormat2E : NetworkFormat
    {
        public override bool Secured => true;
        public override byte Command => 0x2E;
        private Aisling User { get; set; }

        public ServerFormat2E(Aisling user)
        {
            User = user;
        }

        public override void Serialize(NetworkPacketReader reader)
        {

        }

        public override void Serialize(NetworkPacketWriter writer)
        {
            var portal = ServerContext.GlobalWorldMapTemplateCache[User.PortalSession.FieldNumber];
            var name   = string.Format("field{0:000}", portal.FieldNumber);

            writer.WriteStringA(name);
            writer.Write((byte)portal.Portals.Count);
            writer.Write((byte)0x09);

            foreach (var warps in portal.Portals)
            {
                if (warps == null || warps.Destination == null)
                    continue;

                //silly americans!
                writer.Write((short)warps.PointY);
                writer.Write((short)warps.PointX);

                writer.WriteStringA(warps.DisplayName);
                writer.Write((int)warps.Destination.AreaID);
                writer.Write((short)warps.Destination.Location.X);
                writer.Write((short)warps.Destination.Location.Y);
            }
        }
    }
}