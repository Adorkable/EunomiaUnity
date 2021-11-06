using System;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [Serializable]
    public struct SecretCode
    {
        public string Letters;

        [SerializeField] public MatchEvent OnMatch;

        public SecretCode(string letters, MatchEvent onMatch)
        {
            this.Letters = letters;
            this.OnMatch = onMatch;
        }

        public bool Try(string lettersSoFar)
        {
            return this.Letters.StartsWith(lettersSoFar);
        }

        public bool IsMatch(string letters)
        {
            return this.Letters == letters;
        }

        public void Matched()
        {
            this.OnMatch.Invoke();
        }

        [Serializable]
        public class MatchEvent : UnityEvent
        {
        }
    }
}