using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// TODO: only use Unity Random
namespace EunomiaUnity
{
    [Serializable]
    public class Random
    {
        [UnityEngine.SerializeField, HideInInspector]
        private UnityEngine.Random.State unityRandomState;

        // TODO: does it make sense to wrap this way? considering erreyone is static
        public Random(UnityEngine.Random.State unityRandomState)
        {
            // TODO: test if we're re-setting when we deserialize
            this.unityRandomState = unityRandomState;
            UnityEngine.Random.state = this.unityRandomState;
        }

        public static Color ColorRGB
        {
            get
            {
                return new Color(
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    1.0f
                );
            }
        }

        public static Color ColorRGBA
        {
            get
            {
                return new Color(
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f)
                );
            }
        }

        // TODO: use Eunomia's IEnumerable.RandomIndex
        public static int RandomIndex<ItemType>(IEnumerable<ItemType> enumerable)
        {
            var count = enumerable.Count();
            if (count <= 0)
            {
                throw new IndexOutOfRangeException("Provided list is zero length");
            }
            return UnityEngine.Random.Range(0, count);
        }

        // TODO: use Eunomia's IEnumerable.RandomElement
        public static ItemType RandomElement<ItemType>(IEnumerable<ItemType> enumerable)
        {
            if (enumerable.Count() <= 0)
            {
                throw new IndexOutOfRangeException("Provided list is zero length");
            }
            var index = Random.RandomIndex(enumerable);
            return enumerable.ElementAt(index);
        }
    }
}