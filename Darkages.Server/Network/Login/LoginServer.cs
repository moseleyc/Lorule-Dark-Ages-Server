﻿using System.Linq;
using System.Net;
using Darkages.Network.ClientFormats;
using Darkages.Network.ServerFormats;
using Darkages.Security;
using Darkages.Storage;
using Darkages.Types;

namespace Darkages.Network.Login
{
    public class LoginServer : NetworkServer<LoginClient>
    {
        public LoginServer(int capacity)
            : base(capacity)
        {
            MServerTable = MServerTable.FromFile("MServerTable.xml");
            Notification = Notification.FromFile("notification.txt");
        }

        public MServerTable MServerTable { get; set; }
        public Notification Notification { get; set; }

        /// <summary>
        ///     Send Encryption Parameters.
        /// </summary>
        protected override void Format00Handler(LoginClient client, ClientFormat00 format)
        {
            if (format.Version == ServerContext.Config.Version)
                client.Send(new ServerFormat00
                {
                    Type = 0x00,
                    Hash = MServerTable.Hash,
                    Parameters = client.Encryption.Parameters
                });
            else
                client.Send(new ServerFormat00
                {
                    Type = 0x01,
                    Hash = MServerTable.Hash,
                    Parameters = client.Encryption.Parameters
                });
        }

        /// <summary>
        ///     Login Client - Create New Aisling, Choose Username/password.
        /// </summary>
        protected override void Format02Handler(LoginClient client, ClientFormat02 format)
        {
            //save information to memory.
            client.CreateInfo = format;
            client.SendMessageBox(0x00, "\0");
        }

        /// <summary>
        ///     Login Client - Save Character Template.
        /// </summary>
        protected override void Format04Handler(LoginClient client, ClientFormat04 format)
        {
            //make sure the first step was done first.
            if (client.CreateInfo == null)
            {
                ClientDisconnected(client);
                return;
            }

            //create aisling from default template.
            var template = Aisling.Create();
            template.Display = (BodySprite) (format.Gender * 16);
            template.Username = client.CreateInfo.AislingUsername;
            template.Password = client.CreateInfo.AislingPassword;
            template.Gender = (Gender) format.Gender;
            template.HairColor = format.HairColor;
            template.HairStyle = format.HairStyle;


            StorageManager.AislingBucket.Save(template);
            client.SendMessageBox(0x00, "\0");
        }

        /// <summary>
        ///     Login - Check username/password. Proceed to Game Server.
        /// </summary>
        protected override void Format03Handler(LoginClient client, ClientFormat03 format)
        {
            try
            {
                var _aisling = StorageManager.AislingBucket.Load(format.Username);

                if (_aisling != null)
                {
                    if (_aisling.Password != format.Password)
                    {
                        client.SendMessageBox(0x02, "Incorrect Password bud.");
                        return;
                    }
                }
                else
                {
                    client.SendMessageBox(0x02, string.Format("{0} Does not exist.", format.Username));
                    return;
                }
            }
            catch
            {
                client.SendMessageBox(0x02, string.Format("{0} has been blocked.", format.Username));
                return;
            }

            var aislings = GetObjects<Aisling>(i => i.Username == format.Username && format.Password == i.Password);
            foreach (var aisling in aislings)
            {
                aisling.Client.SendMessage(0x02, "You have been replaced by someone else.");
                aisling.Client.Server.ClientDisconnected(aisling.Client);
            }

            var redirect = new Redirect
            {
                Serial = client.Serial,
                Salt = client.Encryption.Parameters.Salt,
                Seed = client.Encryption.Parameters.Seed,
                Name = format.Username
            };

            ServerContext.GlobalRedirects.Add(redirect);

            client.SendMessageBox(0x00, "\0");
            client.Send(new ServerFormat03
            {
                EndPoint = new IPEndPoint(Address, ServerContext.DefaultPort),
                Redirect = redirect
            });
        }

        /// <summary>
        ///     Client Closed Connection (Closed Darkages.exe), Remove them.
        /// </summary>
        protected override void Format0BHandler(LoginClient client, ClientFormat0B format)
        {
            RemoveClient(client);
        }

        /// <summary>
        ///     Redirect Client from Lobby Server to Login Server Automatically.
        /// </summary>
        protected override void Format10Handler(LoginClient client, ClientFormat10 format)
        {
            var redirect = ServerContext.GlobalRedirects.FirstOrDefault(o => o.Serial == format.Id);

            if (redirect == null)
            {
                ClientDisconnected(client);
                return;
            }

            if (redirect.Type == 2)
                ServerContext.Game.RemoveClient(redirect.Client);

            client.Encryption.Parameters = new SecurityParameters(redirect.Seed, redirect.Salt);
            client.Send(new ServerFormat60
            {
                Type = 0x00,
                Hash = Notification.Hash
            });
            ServerContext.GlobalRedirects.Remove(redirect);

            client.SendPacket(new byte[]
            {
                0x19, 0x00, 0xFF, 0x31
            });
        }

        /// <summary>
        ///     Login Client - Update Password.
        /// </summary>
        protected override void Format26Handler(LoginClient client, ClientFormat26 format)
        {
            var _aisling = StorageManager.AislingBucket.Load(format.Username);
            if (_aisling == null)
            {
                client.SendMessageBox(0x02, "Incorrect Information provided.");
                return;
            }

            if (_aisling.Password != format.Password)
            {
                client.SendMessageBox(0x02, "Incorrect Information provided.");
                return;
            }

            if (string.IsNullOrEmpty(format.NewPassword) || format.NewPassword.Length < 3)
            {
                client.SendMessageBox(0x02, "new password not accepted.");
                return;
            }

            //Update new password.
            _aisling.Password = format.NewPassword;
            //Update and Store Information.
            StorageManager.AislingBucket.Save(_aisling);

            client.SendMessageBox(0x00, "\0");
        }

        protected override void Format4BHandler(LoginClient client, ClientFormat4B format)
        {
            client.Send(new ServerFormat60
            {
                Type = 0x01,
                Size = Notification.Size,
                Data = Notification.Data
            });
        }

        protected override void Format57Handler(LoginClient client, ClientFormat57 format)
        {
            if (format.Type == 0x00)
            {
                var redirect = new Redirect
                {
                    Serial = client.Serial,
                    Salt = client.Encryption.Parameters.Salt,
                    Seed = client.Encryption.Parameters.Seed,
                    Name = "socket[" + client.Serial + "]"
                };

                ServerContext.GlobalRedirects.Add(redirect);

                client.Send(new ServerFormat03
                {
                    EndPoint = new IPEndPoint(Address, 2610),
                    Redirect = redirect
                });
            }

            if (format.Type == 0x01)
                client.Send(new ServerFormat56
                {
                    Size = MServerTable.Size,
                    Data = MServerTable.Data
                });
        }

        protected override void Format68Handler(LoginClient client, ClientFormat68 format)
        {
            client.Send(new ServerFormat66());
        }

        protected override void Format7BHandler(LoginClient client, ClientFormat7B format)
        {
            if (format.Type == 0x00)
                client.Send(new ServerFormat6F
                {
                    Type = 0x00,
                    Name = format.Name
                });

            if (format.Type == 0x01)
                client.Send(new ServerFormat6F
                {
                    Type = 0x01
                });
        }

        public override void ClientConnected(LoginClient client)
        {
            client.Send(new ServerFormat7E());
        }
    }
}