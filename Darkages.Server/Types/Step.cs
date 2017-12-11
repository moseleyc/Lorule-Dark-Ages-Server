namespace Darkages.Types
{
    public class Step
    {
        public ushort ScriptId { get; set; }
        public ushort StepId { get; set; }
        public uint Serial { get; set; }

        public bool HasNext { get; set; }
        public bool HasBack { get; set; }

        public string Title { get; set; }
        public string Body { get; set; }
        public ushort Image { get; set; }
    }
}