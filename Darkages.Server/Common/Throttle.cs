using System;
using System.Threading;

namespace Darkages.Common
{
    public class Throttler
    {
        public const long NoLimit = -1;
        private long _consumedTokens;
        private long _lastRefillTime;
        private long _periodTicks;
        private double _averageRate;

        public long BurstSize
        {
            get;
            set;
        }

        public long AverageRate
        {
            get { return (long)_averageRate; }
            set { _averageRate = value; }
        }

        public TimeSpan Period
        {
            get
            {
                return new TimeSpan(_periodTicks);
            }
            set
            {
                _periodTicks = value.Ticks;
            }
        }
        public Throttler()
        {
            BurstSize = 1;
            AverageRate = NoLimit;
            Period = TimeSpan.FromSeconds(1);
        }

        public Throttler(long averageRate, TimeSpan period, long burstSize = 1)
        {
            BurstSize = burstSize;
            AverageRate = averageRate;
            Period = period;
        }

        public bool TryThrottledWait(long amount)
        {
            if (BurstSize <= 0 || _averageRate <= 0)
            { 
                return true;
            }
            RefillToken();
            return ConsumeToken(amount);
        }

        private bool ConsumeToken(long amount)
        {
            while (true)
            {
                long currentLevel = System.Threading.Volatile.Read(ref _consumedTokens);
                if (currentLevel + amount > BurstSize)
                {
                    return false; 
                }

                if (Interlocked.CompareExchange(ref _consumedTokens, currentLevel + amount, currentLevel) == currentLevel)
                {
                    return true;
                }
            }
        }

        public void ThrottledWait(long amount)
        {
            while (true)
            {
                if (TryThrottledWait(amount))
                {
                    break;
                }

                long refillTime = System.Threading.Volatile.Read(ref _lastRefillTime);
                long nextRefillTime = (long)(refillTime + (_periodTicks / _averageRate));
                long currentTimeTicks = DateTime.UtcNow.Ticks;
                long sleepTicks = Math.Max(nextRefillTime - currentTimeTicks, 0);
                TimeSpan ts = new TimeSpan(sleepTicks);
                Thread.Sleep(ts);
            }
        }

        private void RefillToken()
        {
            long currentTimeTicks = DateTime.UtcNow.Ticks;
            long refillTime = System.Threading.Volatile.Read(ref _lastRefillTime);
            long TicksDelta = currentTimeTicks - refillTime;
            long newTokens = (long)(TicksDelta * _averageRate / _periodTicks);

            if (newTokens > 0)
            {
                long newRefillTime = refillTime == 0
                    ? currentTimeTicks
                    : refillTime + (long)(newTokens * _periodTicks / _averageRate);

                if (Interlocked.CompareExchange(ref _lastRefillTime, newRefillTime, refillTime) == refillTime)
                {
                    while (true)
                    {
                        long currentLevel = System.Threading.Volatile.Read(ref _consumedTokens);
                        long adjustedLevel = (long)Math.Min(currentLevel, BurstSize); // In case burstSize decreased
                        long newLevel = (long)Math.Max(0, adjustedLevel - newTokens);
                        if (Interlocked.CompareExchange(ref _consumedTokens, newLevel, currentLevel) == currentLevel)
                        {
                            return;
                        }
                    }
                }
            }
        }
    }
}