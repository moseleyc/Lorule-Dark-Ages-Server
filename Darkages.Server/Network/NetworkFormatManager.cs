using System;

namespace Darkages.Network
{
    public static class NetworkFormatManager
    {
        public static Type[] ClientFormats { get; private set; }
        public static Type[] ServerFormats { get; private set; }

        static NetworkFormatManager()
        {
            NetworkFormatManager.ClientFormats = new Type[256];
            NetworkFormatManager.ServerFormats = new Type[256];

            for (int i = 0; i < 256; i++)
            {
                NetworkFormatManager.ClientFormats[i] = Type.GetType(
                    string.Format("Darkages.Network.ClientFormats.ClientFormat{0:X2}", i), false, false);
                NetworkFormatManager.ServerFormats[i] = Type.GetType(
                    string.Format("Darkages.Network.ServerFormats.ServerFormat{0:X2}", i), false, false);
            }
        }

        public static NetworkFormat GetClientFormat(byte command)
        {
            return Activator.CreateInstance(NetworkFormatManager.ClientFormats[command]) as NetworkFormat;
        }
        public static NetworkFormat GetServerFormat(byte command)
        {
            return Activator.CreateInstance(NetworkFormatManager.ServerFormats[command]) as NetworkFormat;
        }
        public static bool TryGetClientFormat(byte command, out NetworkFormat format)
        {
            return (format = NetworkFormatManager.GetClientFormat(command)) != null;
        }
        public static bool TryGetServerFormat(byte command, out NetworkFormat format)
        {
            return (format = NetworkFormatManager.GetServerFormat(command)) != null;
        }
    }
}