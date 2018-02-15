using Darkages.Storage.locales.Buffs;
using Darkages.Types;

namespace Darkages.Storage.locales.buffs
{
    public class buff_armachd : buff_armor
    {
        public buff_armachd() : base("armachd", 20, 0)
        {

        }

        public override StatusOperator AcModifer => new StatusOperator(StatusOperator.Operator.Remove, 10);

        public override void OnApplied(Sprite Affected, Buff buff)
        {
            if (AcModifer.Option == StatusOperator.Operator.Add)
                Affected.BonusAc += (sbyte)AcModifer.Value;

            else if (AcModifer.Option == StatusOperator.Operator.Remove)
                Affected.BonusAc -= (sbyte)AcModifer.Value;

            if (Affected is Aisling)
            {
                (Affected as Aisling).Client.SendStats(StatusFlags.All);
            }

            base.OnApplied(Affected, buff);
        }

        public override void OnEnded(Sprite Affected, Buff buff)
        {
            if (AcModifer.Option == StatusOperator.Operator.Add)
                Affected.BonusAc -= (sbyte)AcModifer.Value;

            else if (AcModifer.Option == StatusOperator.Operator.Remove)
                Affected.BonusAc += (sbyte)AcModifer.Value;

            if (Affected is Aisling)
            {
                (Affected as Aisling).Client.SendStats(StatusFlags.All);
            }

            base.OnEnded(Affected, buff);
        }
    }
}
