using Darkages.Types;

namespace Darkages.Storage.locales.debuffs
{
    public class debuff_morcradh : debuff_cursed
    {
        public override StatusOperator AcModifer => new StatusOperator(StatusOperator.Operator.Add, 40);
        public debuff_morcradh() : base("mor cradh", 240, 83) { }

        public override void OnApplied(Sprite Affected, Debuff debuff)
        {
            if (AcModifer.Option == StatusOperator.Operator.Add)
                Affected.BonusAc += (sbyte)AcModifer.Value;

            base.OnApplied(Affected, debuff);
        }
        public override void OnEnded(Sprite Affected, Debuff debuff)
        {
            if (AcModifer.Option == StatusOperator.Operator.Add)
                Affected.BonusAc -= (sbyte)AcModifer.Value;

            base.OnEnded(Affected, debuff);
        }

    }
}