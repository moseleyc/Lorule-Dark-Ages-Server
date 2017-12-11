using System.Net;
using System.Xml.Serialization;

namespace Darkages.Types
{
    public class MServer
    {
        [XmlIgnore]
        public IPAddress Address { get; set; }

        [XmlElement("Addr")]
        public string AddressString
        {
            get { return this.Address.ToString(); }
            set { this.Address = IPAddress.Parse(value); }
        }
        [XmlElement("Port")]
        public ushort Port { get; set; }
        [XmlElement("Guid")]
        public byte Guid { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("Desc")]
        public string Description { get; set; }

        public MServer()
        {
        }

        public MServer(byte guid, string name, string description, IPAddress address, ushort port)
        {
            Guid = guid;
            Name = name;
            Description = description;
            Address = address;
            Port = port;
        }
    }
}