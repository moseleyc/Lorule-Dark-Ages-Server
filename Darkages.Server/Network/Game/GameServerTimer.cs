using System;

namespace Darkages.Network.Game
{
    public class GameServerTimer
    {
        public TimeSpan Timer { get; set; }
        public TimeSpan Delay { get; set; }

        public bool Elapsed
        {
            get { return (this.Timer >= this.Delay); }
        }

        public bool Disabled { get; set; }
        public int Interval { get; set; }
        public int Tick { get; set; }

        public GameServerTimer(TimeSpan delay)
        {
            this.Timer = TimeSpan.Zero;
            this.Delay = delay;
        }

        public void Reset()
        {
            this.Timer = TimeSpan.Zero;
        }
        public void Update(TimeSpan elapsedTime)
        {
            this.Timer += elapsedTime;
        }
    }
}