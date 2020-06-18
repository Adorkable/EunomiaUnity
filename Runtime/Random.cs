using System;
using System.Collections.Generic;
using UnityEngine;

// TODO: only use Unity Random
namespace EunomiaUnity {
    [Serializable]
    public class Random {
        [UnityEngine.SerializeField, HideInInspector]
        private UnityEngine.Random.State unityRandomState;

        [UnityEngine.SerializeField, HideInInspector]
        public Eunomia.WordList NounList { get; private set; }

        // TODO: does it make sense to wrap this way? considering erreyone is static
        public Random(UnityEngine.Random.State unityRandomState, Eunomia.WordList nounList) {
            // TODO: test if we're re-setting when we deserialize
            this.unityRandomState = unityRandomState;
            UnityEngine.Random.state = this.unityRandomState;

            this.NounList = nounList;
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
            get {
                return new Color(
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f),
                    UnityEngine.Random.Range(0.0f, 1.0f)
                );
            }
        }

        public static int RandomIndex<ItemType>(List<ItemType> list) {
            if (list.Count <= 0) {
                throw new IndexOutOfRangeException("Provided list is zero length");
            }
            return UnityEngine.Random.Range(0, list.Count);
        }

        public static ItemType RandomElement<ItemType>(List<ItemType> list) {
            if (list.Count <= 0) {
                throw new IndexOutOfRangeException("Provided list is zero length");
            }
            return list[Random.RandomIndex(list)];
        }

        public string Noun {
            get {
                var nouns = this.NounList.All;
                if (nouns == null || nouns.Count <= 0) {
                    throw new IndexOutOfRangeException("No nouns found");
                }

                return EunomiaUnity.Random.RandomElement(nouns);
            }
        }

        public string NounWithLength(int length) {
            var nounsWithLength = this.NounList.WithLength(length);
            if (nounsWithLength == null || nounsWithLength.Count <= 0) {
                throw new IndexOutOfRangeException("No nouns found with length " + length);
            }

            return EunomiaUnity.Random.RandomElement(nounsWithLength);
        }
    }
}