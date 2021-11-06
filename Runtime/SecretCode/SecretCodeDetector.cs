using System;
using System.Collections.Generic;
using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public interface IGetKeyDownSource
    {
        bool GetKeyDown(KeyCode key);
    }

    [Serializable]
    public class SecretCodeDetector : MonoBehaviour
    {
        public SecretCode[] secretCodes;

        public float MaximumBetweenSeconds = 10;

        protected List<Attempt> attempts;

        public IGetKeyDownSource input = new InputWrapper();
        protected float TimeSinceLastMatchSeconds;

        private void Awake()
        {
            this.attempts = new List<Attempt>();
        }

        private void Update()
        {
            this.TimeSinceLastMatchSeconds += Time.deltaTime;

            if (this.TimeSinceLastMatchSeconds >= this.MaximumBetweenSeconds && this.attempts.Count > 0)
            {
                this.attempts.Clear();

                return;
            }

            // TODO: ouch, is this really going through all KeyCodes every frame??
            // ----: find a faster way to get all keys down at the moment
            // ----: we can just test the next key or any other key
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (this.GetKeyDown(key))
                {
                    this.TestMatches(key);
                }
            }
        }

        public void Add(SecretCode code)
        {
            this.secretCodes = Arrays.AddToBack(code, this.secretCodes);
        }

        protected bool GetKeyDown(KeyCode key)
        {
            if (this.input == null)
            {
                this.input = new InputWrapper();
            }

            return this.input.GetKeyDown(key);
        }

        private void TestMatches(KeyCode key)
        {
            var index = 0;
            while (index < this.attempts.Count)
            {
                var attempt = this.attempts[index];
                if (attempt.Try(key))
                {
                    this.TimeSinceLastMatchSeconds = 0;

                    if (attempt.IsMatched())
                    {
                        attempt.SecretCode.Matched();
                        this.attempts.RemoveAt(index);
                    }
                    else
                    {
                        index++;
                    }
                }
                else
                {
                    this.attempts.RemoveAt(index);
                }
            }

            foreach (var code in this.secretCodes)
            {
                if (code.Letters.StartsWith(key.ToString()))
                {
                    var attempt = new Attempt(code);
                    attempt.Try(key);
                    this.attempts.Add(attempt);
                }
            }
        }

        private class InputWrapper : IGetKeyDownSource
        {
            public bool GetKeyDown(KeyCode key)
            {
                return Input.GetKeyDown(key);
            }
        }

        protected class Attempt
        {
            public Attempt(SecretCode secretCode)
            {
                this.SecretCode = secretCode;
            }

            public SecretCode SecretCode { get; protected set; }
            public string LettersSoFar { get; protected set; }

            public bool Try(KeyCode key)
            {
                var lettersSoFar =
                    $"{this.LettersSoFar}{key.ToString()}"; // TODO: key isn't safe to use directly, gotta translate
                if (!this.SecretCode.Try(lettersSoFar))
                {
                    return false;
                }

                this.LettersSoFar = lettersSoFar;
                return true;
            }

            public bool IsMatched()
            {
                return this.SecretCode.IsMatch(this.LettersSoFar);
            }
        }
    }
}