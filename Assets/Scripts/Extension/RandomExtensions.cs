﻿using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Extension
{
    static class RandomExtensions
    {
        public delegate float WeightSelector<in T>(T element);

        public static T GetRandomElementOrDefault<T>(this IEnumerable<T> sequence)
        {
            return sequence.GetWeightedRandomElementOrDefault(p => 1);
        }
        public static T GetWeightedRandomElementOrDefault<T>(this IEnumerable<T> sequence, WeightSelector<T> weightSelector)
        {
            sequence = sequence.Where(element => weightSelector(element) > 0);
            var totalWeight = sequence.Sum(element => weightSelector(element));
            var requestedElement = UnityEngine.Random.Range(0, totalWeight);
            foreach (var element in sequence)
            {
                var elementWeight = weightSelector(element);
                if (requestedElement <= elementWeight)
                {
                    return element;
                }
                requestedElement -= elementWeight;
            }
            UnityEngine.Debug.Assert(!sequence.Any(), "We should always get a result if the sequence is not empty.");
            return default;
        }
    }
}