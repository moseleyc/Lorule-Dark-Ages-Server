using System;
using System.Collections.Generic;
using System.Linq;
using Darkages.Systems.Loot.Interfaces;

namespace Darkages.Systems.Loot.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random Random = new Random();
        private static Dictionary<double, long> GlobalDraws = new Dictionary<double, long>();

        static float NextFloat(Random random)
        {
            double mantissa = (random.NextDouble() * 2.0) - 1.0;
            double exponent = Math.Pow(2.0, random.Next(-126, 128));
            return (float)(mantissa * exponent);
        }

        public static T WeightedChoice<T>(this IEnumerable<T> items, double sum) where T : IWeighable
        {
            var randomNumber = Random.Next(0, items.Count());
            var objs = items.ToArray();

            foreach (var item in items)
            {
                lock (Random)
                {
                    short luck   = (short)Math.Abs(NextFloat(Random));

                    if (luck <= -19000)
                    {
                        return objs[randomNumber];
                    }
                }
            }

            return default(T);
        }
    }
}