using System;
using System.Net;

namespace Darkages.Network
{
    public class IPAttribute : Attribute
    {
        public IPAddress EndPoint { get; set; }

        public IPAttribute(string IP)
        {
            EndPoint = IPAddress.Parse(IP);
        }
    }
}