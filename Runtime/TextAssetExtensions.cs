using System.Linq;
using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class TextAssetExtensions
    {
        public static WordList ToWordList(this TextAsset from)
        {
            var allWords = from.text.Split('\n').ToList();
            return new WordList(allWords);
        }
    }
}