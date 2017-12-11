using System.Collections.Generic;

namespace Darkages.Types
{
    public class NormalPopup : Popup
    {
        public List<Step> Steps = new List<Step>();

        public NormalPopup()
        {
            TotalSteps  = Steps.Count;
            CurrentStep = 0;
        }
    }
}