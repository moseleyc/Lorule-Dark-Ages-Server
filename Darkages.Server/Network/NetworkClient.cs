using Darkages.Network.Game;
using Darkages.Network.Object;
using Darkages.Network.ServerFormats;
using Darkages.Security;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Darkages.Network
{
    [Serializable]
    public class NetworkClient : ObjectManager
    {
        public NetworkPacketReader Reader { get; set; }
        public NetworkPacketWriter Writer { get; set; }
        public NetworkSocket Socket { get; set; }
        public SecurityProvider Encryption { get; set; }
        public byte Ordinal { get; set; }
        public int Serial { get; set; }
        public bool Running { get; set; }

        public NetworkClient()
        {
            this.Reader = new NetworkPacketReader();
            this.Writer = new NetworkPacketWriter();
            this.Encryption = new SecurityProvider();
        }

        private static byte P(NetworkPacket value) => (byte)(value.Data[1] ^ (byte)(value.Data[0] - 0x2D));

        private static void TransFormDialog(NetworkPacket value) 
        {
            value.Data[2] ^= (byte)(P(value) + 0x72);
            value.Data[3] ^= (byte)(((byte)(P(value) + 0x72) + 1) % 256);
            value.Data[4] ^= (byte)(P(value) + 0x28);
            value.Data[5] ^= (byte)(((byte)(P(value) + 0x28) + 1) % 256);
            Parallel.For(0, value.Data.Length - 6, i => value.Data[6 + i] ^= (byte)(((byte)(P(value) + 0x28) + i + 2) % 256));
        }


        public void Read(NetworkPacket packet, NetworkFormat format)
        {
            try
            {
                if (format.Secured)
                {
                    Encryption.Transform(packet);

                    if (format.Command == 0x39 || format.Command == 0x3A)
                    {
                        TransFormDialog(packet);
                        Reader.Position = 6;
                    }
                    else
                    {
                        Reader.Position = 0;
                    }
                }
                else
                {
                    Reader.Position = -1;
                }

                Reader.Packet = packet;

                if (ServerContext.Config.LogRecvPackets)
                {
                    if (this is GameClient)
                    {
                        Console.WriteLine("{0}: {1}", (this as GameClient).Aisling?.Username, packet.ToString());
                    }
                }

                format.Serialize(Reader);
            }
            catch
            {

            }
        }


        private byte LastFormat;
        private int Matches = 0;

        private ManualResetEvent sendDone = new ManualResetEvent(!ServerContext.Config.SendClientPacketsAsAsync);
        private Queue<NetworkFormat> _sendBuffers = new Queue<NetworkFormat>();

        private bool _sending;

        public void SendAsync(NetworkFormat format)
        {
            lock (_sendBuffers)
            {
                _sendBuffers.Enqueue(format);

                if (_sending)
                    return;

                _sending = true;
                ThreadPool.QueueUserWorkItem(SendBuffers);
            }
        }

        private void SendBuffers(object state)
        {
            while (true)
            {
                try
                {
                    NetworkFormat format;

                    lock (_sendBuffers)
                    {
                        if (_sendBuffers.Count == 0)
                        {
                            _sending = false;
                            return;
                        }

                        format = _sendBuffers.Dequeue();
                    }

                    SendFormat(format);
                }
                catch
                {
                    return;
                }
            }
        }

        private void SendFormat(NetworkFormat format)
        {
            if (format == null)
                return;

            this.Writer.Position = 0;
            this.Writer.Write(format.Command);

            if (format.Secured)
            {
                this.Writer.Write(this.Ordinal++);
            }

            format.Serialize(this.Writer);

            var packet = this.Writer.ToPacket();
            var SendIt = true;

            if (LastFormat == format.Command)
            {
                ++Matches;
            }
            else
            {
                LastFormat = format.Command;
                Matches = 0;
            }

            SendIt = Matches < ((format is ServerFormat3C) ? ServerContext.Config.PacketOverflowLimit
                : ServerContext.Config.ServerOverflowTolerate);

            if (ServerContext.Config.LogSentPackets)
            {
                if (this is GameClient)
                {
                    Console.WriteLine("{0}: {1}", (this as GameClient).Aisling?.Username, packet.ToString());
                }
            }

            if (format.Secured)
            {
                this.Encryption.Transform(packet);
            }

            if (SendIt)
            {
                var buffer = packet.ToArray();

                if (ServerContext.Config.SendClientPacketsAsAsync)
                {
                    Socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(SendCallback), Socket);
                    return;
                }

                Socket.Send(buffer);
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;
                int bytesSent = client.EndSend(ar);

                sendDone.Set();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public int errors = 0;

        public void Send(NetworkFormat format)
        {
            var queue = ServerContext.Config.QueuePackets;

            @catcher:
            try
            {
                if (queue)
                {
                    SendAsync(format);
                }
                else
                {
                    SendFormat(format);
                }

                errors = 0;
            }
            catch
            {
                if (errors++ <= ServerContext.Config.ClientPacketSendErrorLimit)
                {
                    Writer = new NetworkPacketWriter();
                    queue = !queue;

                    goto catcher;
                }
                else
                {
                    return;
                }
            }
        }

        public void SendMessageBox(byte code, string text)
        {
            this.Send(new ServerFormat02(code, text)); 
        }
    }
}