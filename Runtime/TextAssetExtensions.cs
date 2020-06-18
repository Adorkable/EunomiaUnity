using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EunomiaUnity {
    public static class TextAssetExtensions {
        public static Eunomia.WordList ToWordList(this TextAsset from) {
            List<string> allWords = from.text.Split('\n').ToList();
            return new Eunomia.WordList(allWords);
        }
    }
}