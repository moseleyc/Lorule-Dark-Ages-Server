using Darkages.Types;
using System.Collections.Generic;
using System.Linq;

namespace Darkages.Network.ServerFormats
{
    public class ServerFormat2F : NetworkFormat
    {
        public override bool Secured
        {
            get { return true; }
        }
        public override byte Command
        {
            get { return 0x2F; }
        }

        public IDialogData Data { get; set; }
        public Mundane Mundane { get; set; }
        public string Text { get; set; }

        public ServerFormat2F(Mundane mundane, string text, IDialogData data)
        {
            this.Mundane = mundane;
            this.Text = text;
            this.Data = data;
        }

        public override void Serialize(NetworkPacketReader reader)
        {
        }
        public override void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((byte)this.Data.Type);
            writer.Write((byte)0x01);
            writer.Write((uint)this.Mundane.Serial);
            writer.Write((byte)0x02);

            writer.Write((ushort)Mundane.Template.Image);
            writer.Write((byte)0x00);

            writer.Write((byte)0x01);
            writer.Write((byte)0x02);
            writer.Write((byte)0x01);

            writer.Write(byte.MinValue);

            writer.WriteStringB(Mundane.Template.Name);
            writer.WriteStringB(this.Text);
            writer.Write(this.Data);
        }
    }

    public interface IDialogData : IFormattable
    {
        byte Type { get; }
    }

    public class OptionsDataItem
    {
        public string Text { get; set; }
        public short Step { get; set; }

        public OptionsDataItem(short step, string text)
        {
            this.Step = step;
            this.Text = text;
        }
    }

    public class OptionsData : List<OptionsDataItem>, IDialogData
    {
        public byte Type
        {
            get { return 0x00; }
        }

        public OptionsData(IEnumerable<OptionsDataItem> collection)
            : base(collection)
        {
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(
                (byte)this.Count);

            foreach (var option in this)
            {
                writer.WriteStringA(option.Text);
                writer.Write((short)option.Step);
            }
        }
    }
    public class OptionsPlusArgsData : List<OptionsDataItem>, IDialogData
    {
        public byte Type
        {
            get { return 0x01; }
        }
        public string Args { get; set; }

        public OptionsPlusArgsData(IEnumerable<OptionsDataItem> collection, string args)
            : base(collection)
        {
            this.Args = args;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.WriteStringA(this.Args);
            writer.Write(
                (byte)this.Count);

            foreach (var option in this)
            {
                writer.WriteStringA(option.Text);
                writer.Write((short)option.Step);
            }
        }
    }
    public class TextInputData : IDialogData
    {
        public byte Type
        {
            get { return 0x02; }
        }
        public ushort Step { get; set; }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(this.Step);
        }
    }
    public class ItemShopData : IDialogData
    {
        public byte Type
        {
            get { return 0x04; }
        }
        public IEnumerable<ItemTemplate> Items { get; set; }
        public ushort Step { get; set; }

        public ItemShopData(ushort step, IEnumerable<ItemTemplate> items)
        {
            this.Step = step;
            this.Items = items;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {

            writer.Write((ushort)Step);
            writer.Write((ushort)Items.Count());

            foreach (var item in Items)
            {
                writer.Write((ushort)item.DisplayImage);
                writer.Write((byte)0x00);
                writer.Write((uint)item.Value);
                writer.WriteStringA(item.Name);
                writer.WriteStringA("");
            }
        }
    }
    public class ItemSellData : IDialogData
    {
        public byte Type
        {
            get { return 0x05; }
        }
        public IEnumerable<byte> Items { get; set; }
        public ushort Step { get; set; }

        public ItemSellData(ushort step, IEnumerable<byte> items)
        {
            this.Step = step;
            this.Items = items;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(this.Step);
            writer.Write(
                (short)this.Items.Count());
            writer.Write(this.Items.ToArray());
        }
    }
    public class SpellAcquireData : IDialogData
    {
        public byte Type
        {
            get { return 0x06; }
        }
        public IEnumerable<SpellTemplate> Spells { get; set; }
        public ushort Step { get; set; }

        public SpellAcquireData(ushort step, IEnumerable<SpellTemplate> spells)
        {
            this.Step = step;
            this.Spells = spells;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((ushort)this.Step);
            writer.Write(
                (ushort)this.Spells.Count());

            foreach (var spell in this.Spells)
            {
                writer.Write((byte)0x02);
                writer.Write((ushort)spell.Icon);
                writer.Write((byte)0x00);
                writer.WriteStringA(spell.Name);
            }

        }
    }
    public class SkillAcquireData : IDialogData
    {
        public byte Type
        {
            get { return 0x07; }
        }
        public IEnumerable<SkillTemplate> Skills { get; set; }
        public ushort Step { get; set; }

        public SkillAcquireData(ushort step, IEnumerable<SkillTemplate> skills)
        {
            this.Step = step;
            this.Skills = skills;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write((ushort)this.Step);
            writer.Write(
                (ushort)this.Skills.Count());

            foreach (var skill in this.Skills)
            {
                writer.Write((byte)0x03);
                writer.Write((ushort)skill.Icon);
                writer.Write((byte)0x00);
                writer.WriteStringA(skill.Name);
            }
        }
    }
    public class SpellForfeitData : IDialogData
    {
        public byte Type
        {
            get { return 0x08; }
        }
        public ushort Step { get; set; }

        public SpellForfeitData(ushort step)
        {
            this.Step = step;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(this.Step);
        }
    }
    public class SkillForfeitData : IDialogData
    {
        public byte Type
        {
            get { return 0x09; }
        }
        public ushort Step { get; set; }

        public SkillForfeitData(ushort step)
        {
            this.Step = step;
        }

        public void Serialize(NetworkPacketReader reader)
        {
        }
        public void Serialize(NetworkPacketWriter writer)
        {
            writer.Write(this.Step);
        }
    }
}

