using System;
using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity
{
    public static class FakeLocationGenerator
    {
        public static float GenerateNormalizedEnterValue(bool allow0 = true, bool allow1 = true)
        {
            List<float> possibilities = new List<float>();
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
                throw new ArgumentOutOfRangeException("allow 0 and allow 1 cannot both be false");
            }

            return EunomiaUnity.Random.RandomElement(possibilities);
        }

        public static Vector2 GenerateNormalizedEnterLocation(bool x0 = true, bool x1 = true, bool y0 = true, bool y1 = true)
        {
            return new Vector2(
                GenerateNormalizedEnterValue(x0, x1),
                GenerateNormalizedEnterValue(y0, y1)
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
