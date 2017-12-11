using Darkages.Types;
using System;
using System.Collections.Generic;
using System.Net;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat34 : NetworkFormat
    {
        private enum Slots : byte
        {
            Weapon = 1,
            Armor  = 2,
            Shield = 3,
            Head   = 5,
            Ears   = 6,
            Neck   = 7,
            Ring_A = 8,
            Ring_B = 9,
            Gauntlet_A = 10,
            Gauntlet_B = 11,
            Belt = 12,
            Legs = 13,
            Accessory_1 = 14,
            Foot = 15,
            Body_Formal = 16,
            Accessory_2 = 17,
            Head_Formal = 18,
        }

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
                return 0x34;
            }
        }

        public Aisling Aisling { get; set; }

        public ServerFormat34(Aisling aisling)
        {
            Aisling = aisling;
        }

        public override void Serialize(NetworkPacketReader reader)
        {

        }



        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((uint)Aisling.Serial);

            BuildEquipment(writer);

            writer.Write((byte)ActivityStatus.Awake);
            writer.WriteStringA(Aisling.Username);
            writer.Write((byte)Aisling.Nation);
            writer.WriteStringA(Aisling.Stage.ToString());
            writer.Write((byte)0x00);

            writer.WriteStringA(Aisling.ClanTitle);
            writer.WriteStringA(Aisling.Path.ToString());
            writer.WriteStringA(string.Empty);

            writer.Write((byte)Aisling.LegendBook.LegendMarks.Count);
            foreach (var mark in Aisling.LegendBook.LegendMarks)
            {
                writer.Write((byte)mark.Icon);
                writer.Write((byte)mark.Color);
                writer.WriteStringA(mark.Category);
                writer.WriteStringA(mark.Value);
            }
        }

        private void BuildEquipment(NetworkPacketWriter writer)
        {
            //1
            if (Aisling.EquipmentManager.Weapon != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Weapon.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //2
            if (Aisling.EquipmentManager.Armor != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Armor.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //3
            if (Aisling.EquipmentManager.Shield != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Shield.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //4
            if (Aisling.EquipmentManager.DisplayHelm != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.DisplayHelm.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //5
            if (Aisling.EquipmentManager.Earring != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Earring.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //6
            if (Aisling.EquipmentManager.Necklace != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Necklace.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //7
            if (Aisling.EquipmentManager.LRing != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.LRing.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //8
            if (Aisling.EquipmentManager.RRing != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.RRing.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //9
            if (Aisling.EquipmentManager.LGauntlet != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.LGauntlet.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //10
            if (Aisling.EquipmentManager.RGauntlet != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.RGauntlet.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //11
            if (Aisling.EquipmentManager.Belt != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Belt.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //12
            if (Aisling.EquipmentManager.Greaves != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Greaves.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //13
            if (Aisling.EquipmentManager.FirstAcc != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.FirstAcc.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            if (Aisling.EquipmentManager.Boots != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Boots.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //15
            if (Aisling.EquipmentManager.Overcoat != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Overcoat.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //16
            if (Aisling.EquipmentManager.Helmet != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.Helmet.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }

            //17
            if (Aisling.EquipmentManager.SecondAcc != null)
            {
                writer.Write((ushort)Aisling.EquipmentManager.SecondAcc.Item.DisplayImage);
                writer.Write((byte)0x00);
            }
            else
            {
                writer.Write(ushort.MinValue);
                writer.Write((byte)0x00);
            }



        }
    }
}
