using Darkages.Types;

namespace Darkages.Storage.locales.Buffs
{
    public class buff_armor : Buff
    {
        public buff_armor(string name, int length, byte icon)
        {
            Name = name;
            Length = length;
            Icon = icon;
        }

        public virtual StatusOperator AcModifer { get; set; }

        public override void OnApplied(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "Your armor has been increased.");

            base.OnApplied(Affected, buff);
        }

        public override void OnDurationUpdate(Sprite Affected, Buff buff)
        {
            base.OnDurationUpdate(Affected, buff);
        }

        public override void OnEnded(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "Your armor returns to normal.");

            base.OnEnded(Affected, buff);
        }
    }
}