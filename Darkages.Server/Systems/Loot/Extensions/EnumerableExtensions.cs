using System;
using System.Collections.Generic;
using Darkages.Systems.Loot.Interfaces;

namespace Darkages.Systems.Loot.Extensions
{
    public static class EnumerableExtensions
    {
        private static readonly Random Random = new Random();

        public static T WeightedChoice<T>(this IEnumerable<T> items, int sum) where T : IWeighable
        {
            var randomNumber = Random.Next(0, sum + 1);

            foreach (var item in items)
            {
                randomNumber -= item.Weight;

                if (!(randomNumber <= 0))
                    continue;

                return item;
            }

            throw new NullReferenceException($"Unable to get an item from the list of items.");
        }
    }
}