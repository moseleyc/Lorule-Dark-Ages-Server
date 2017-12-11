using Darkages.Network.Game;
using System;
using System.Diagnostics;
using System.Net.Sockets;

namespace Darkages.Network
{
    public class NetworkSocket : Socket
    {
        private static readonly int processId = Process.GetCurrentProcess().Id;

        private byte[] header = new byte[0x0003];
        private byte[] packet = new byte[ServerContext.Config.BufferSize];
        private int headerLength = 3;
        private int headerOffset = 0;
        private int packetLength = 0;
        private int packetOffset = 0;

        public bool HeaderComplete
        {
            get { return (this.headerOffset == this.headerLength); }
        }
        public bool PacketComplete
        {
            get { return (this.packetOffset == this.packetLength); }
        }

        public NetworkSocket(Socket socket)
            : base(socket.DuplicateAndClose(processId))
        {

        }

        public IAsyncResult BeginReceiveHeader(AsyncCallback callback, out SocketError error, object state)
        {
            return base.BeginReceive(
                this.header,
                this.headerOffset,
                this.headerLength - this.headerOffset,
                SocketFlags.None,
                out error,
                callback,
                state);
        }
        public IAsyncResult BeginReceivePacket(AsyncCallback callback, out SocketError error, object state)
        {
            return base.BeginReceive(
                this.packet,
                this.packetOffset,
                this.packetLength - this.packetOffset,
                SocketFlags.None,
                out error,
                callback,
                state);
        }
        public int EndReceiveHeader(IAsyncResult result, out SocketError error)
        {
            var bytes = base.EndReceive(result, out error);

            if (bytes == 0 ||
                error != SocketError.Success)
                return 0;

            this.headerOffset += bytes;

            if (this.HeaderComplete)
            {
                this.packetLength = (this.header[1] << 8) | this.header[2];
                this.packetOffset = (0);
            }

            return bytes;
        }
        public int EndReceivePacket(IAsyncResult result, out SocketError error)
        {
            var bytes = base.EndReceive(result, out error);

            if (bytes == 0 ||
                error != SocketError.Success)
                return 0;

            this.packetOffset += bytes;

            if (this.PacketComplete)
            {
                this.headerLength = (3);
                this.headerOffset = (0);
            }

            return bytes;
        }

        public NetworkPacket ToPacket()
        {
            return (this.PacketComplete) ?
                new NetworkPacket(this.packet, 0, this.packetLength) :
                null;
        }
    }
}