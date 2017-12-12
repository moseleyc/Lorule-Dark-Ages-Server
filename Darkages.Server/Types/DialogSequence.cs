namespace Darkages.Types
{
    public class DialogSequence
    {
        public string Title { get; set; }
        public string DisplayText { get; set; }
        public bool HasOptions { get; set; }
        public bool StartsQuest { get; set; }

        public void OnClose()
        {
        }
    }
}
