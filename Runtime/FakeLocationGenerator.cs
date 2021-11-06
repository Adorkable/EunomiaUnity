using System;
using System.Collections.Generic;
using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class FakeLocationGenerator
    {
        public static float GenerateNormalizedEnterValue(Random random, bool allow0 = true, bool allow1 = true)
        {
            var possibilities = new List<float>();
            if (allow0)
            {
                possibilities.Add(0);
            }

            if (allow1)
            {
                possibilities.Add(1);
            }

            if (possibilities.Count == 0)
            {
                throw new ArgumentException("allow 0 and allow 1 cannot both be false");
            }

            return possibilities.RandomElement(random);
        }

        public static Vector2 GenerateNormalizedEnterLocation(Random random, bool x0 = true, bool x1 = true,
            bool y0 = true, bool y1 = true)
        {
            return new Vector2(
                GenerateNormalizedEnterValue(random, x0, x1),
                GenerateNormalizedEnterValue(random, y0, y1)
            );
        }

        public static Vector2 GenerateNormalizedOffsetLocation(Vector2 original, float minimumSpeed, float maximumSpeed)
        {
            return Vector2.MoveTowards(
                original,
                new Vector2(UnityEngine.Random.Range(0, 1), UnityEngine.Random.Range(0, 1)),
                UnityEngine.Random.Range(minimumSpeed, maximumSpeed) * Time.deltaTime
            );
        }
    }
}