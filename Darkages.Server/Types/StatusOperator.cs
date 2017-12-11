namespace Darkages.Types
{
    public class StatusOperator
    {
        public enum Operator
        {
            Add = 0,
            Remove = 1,
        }

        public Operator Option { get; set; }
        public int Value { get; set; }

        public StatusOperator(Operator option, int value)
        {
            this.Option = option;
            this.Value  = value;
        }
    }
}
