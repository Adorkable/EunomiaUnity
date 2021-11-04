using System;
using System.Collections.Generic;
using Eunomia;
using UnityEngine;

namespace EunomiaUnity
{
    public interface GetKeyDownSource
    {
        bool GetKeyDown(KeyCode key);
    }

    [Serializable]
    public class SecretCodeDetector : MonoBehaviour
    {
        private class InputWrapper : GetKeyDownSource
        {
            public bool GetKeyDown(KeyCode key)
            {
                return Input.GetKeyDown(key);
            }
        }

        public GetKeyDownSource input = new InputWrapper();
        public SecretCode[] secretCodes;

        protected class Attempt
        {
            public SecretCode SecretCode { get; protected set; }
            public string LettersSoFar { get; protected set; }

            public Attempt(SecretCode secretCode)
            {
                this.SecretCode = secretCode;
            }

            public bool Try(KeyCode key)
            {
                var lettersSoFar = this.LettersSoFar + key.ToString();
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
        protected List<Attempt> attempts;

        public float MaximumBetweenSeconds = 10;
        protected float TimeSinceLastMatchSeconds;

        void Awake()
        {
            this.attempts = new List<Attempt>();
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

        void Update()
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
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (this.GetKeyDown(key))
                {
                    this.TestMatches(key);
                }
            }
        }

        void TestMatches(KeyCode key)
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

            foreach (SecretCode code in this.secretCodes)
            {
                if (code.Letters.StartsWith(key.ToString()))
                {
                    var attempt = new Attempt(code);
                    attempt.Try(key);
                    this.attempts.Add(attempt);
                }
            }
        }
    }
}