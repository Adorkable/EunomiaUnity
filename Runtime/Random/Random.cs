using System;
using System.Collections.Generic;
using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [Serializable]
    public class Random : IRandom
    {
        protected IRandom random;

        public Random(IRandom random)
        {
            this.random = random;
        }

        int IRandom.Next()
        {
            return random.Next();
        }

        int IRandom.Next(int maxValue)
        {
            return random.Next(maxValue);
        }

        int IRandom.Next(int minValue, int maxValue)
        {
            return random.Next(minValue, maxValue);
        }

        public float Next()
        {
            var result = random.Next();
            return ((float)result).Map(0, int.MaxValue, 0, 1);
        }

        public float Next(float maxValue)
        {
            var result = random.Next();
            return ((float)result).Map(0, int.MaxValue, 0, maxValue);
        }

        public float Next(float minValue, float maxValue)
        {
            var result = random.Next();
            return ((float)result).Map(0, int.MaxValue, minValue, maxValue);
        }

        public Color ColorRGB()
        {
            return new Color(
                Next(0.0f, 1.0f),
                Next(0.0f, 1.0f),
                Next(0.0f, 1.0f),
                1.0f
            );
        }

        public Color ColorRGBA()
        {
            return new Color(
                Next(0.0f, 1.0f),
                Next(0.0f, 1.0f),
                Next(0.0f, 1.0f),
                Next(0.0f, 1.0f)
            );
        }

        public int RandomIndex<ItemType>(IEnumerable<ItemType> enumerable)
        {
            return enumerable.RandomIndex(this);
        }

        public ItemType RandomElement<ItemType>(IEnumerable<ItemType> enumerable)
        {
            return enumerable.RandomElement(this);
        }
    }
}