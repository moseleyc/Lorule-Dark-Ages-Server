using Darkages.Types;

namespace Darkages.Storage.locales.Buffs
{
    public class buff_dion : Buff
    {
        /// <summary>
        /// This name MUST match and correspond the name in the type BUFF.
        /// </summary>
        public override string Name
        {
            get => "dion";
        }

        public override int  Length => 6;
        public override byte Icon   => 19;

        public buff_dion()
        {

        }

        public override void OnApplied(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
            {
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "Your skin turns to stone.");
            }

            base.OnApplied(Affected, buff);
        }

        public override void OnDurationUpdate(Sprite Affected, Buff buff)
        {
            base.OnDurationUpdate(Affected, buff);
        }

        public override void OnEnded(Sprite Affected, Buff buff)
        {
            if (Affected is Aisling)
            {
                (Affected as Aisling)
                    .Client
                    .SendMessage(0x02, "Your skin turns back to flesh.");
            }

            base.OnEnded(Affected, buff);
        }
    }
}