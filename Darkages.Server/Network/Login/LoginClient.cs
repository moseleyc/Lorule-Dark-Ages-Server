using Darkages.Network.ClientFormats;

namespace Darkages.Network.Login
{
    public class LoginClient : NetworkClient
    {
        public ClientFormat02 CreateInfo { get; set; }
    }
}