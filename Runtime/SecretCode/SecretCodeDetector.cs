using System;
using System.Collections.Generic;
using Eunomia;
using UnityEngine;

namespace EunomiaUnity {
    [Serializable]
    public class SecretCodeDetector : MonoBehaviour {
        public SecretCode[] secretCodes;

        protected class Attempt {
            public SecretCode SecretCode { get; protected set; }
            public string LettersSoFar { get; protected set; }

            public Attempt(SecretCode secretCode) {
                this.SecretCode = secretCode;
            }

            public bool Try(KeyCode key) {
                var lettersSoFar = this.LettersSoFar + key.ToString();
                if (!this.SecretCode.Try(lettersSoFar)) {
                    return false;
                }
                this.LettersSoFar = lettersSoFar;
                return true;
            }

            public bool IsMatched() {
                return this.SecretCode.IsMatch(this.LettersSoFar);
            }
        }
        protected List<Attempt> attempts;

        public float MaximumBetweenSeconds = 10;
        protected float TimeSinceLastMatchSeconds;

        void Awake() {
            this.attempts = new List<Attempt>();
        }

        public void Add(SecretCode code) {
            this.secretCodes = Arrays.AddToBack(code, this.secretCodes);
        }

        void Update() {
            if (this.TimeSinceLastMatchSeconds >= this.MaximumBetweenSeconds && this.attempts.Count > 0) {
                this.attempts.Clear();

                return;
            }

            this.TimeSinceLastMatchSeconds += Time.deltaTime;
            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode))) {
                if (Input.GetKeyDown(key)) {
                    this.TestMatches(key);
                }
            }
        }

        void TestMatches(KeyCode key) {
            var index = 0;
            while (index < this.attempts.Count) {
                var attempt = this.attempts[index];
                if (attempt.Try(key)) {
                    this.TimeSinceLastMatchSeconds = 0;

                    if (attempt.IsMatched()) {
                        attempt.SecretCode.Matched();
                        this.attempts.RemoveAt(index);
                    } else {
                        index++;
                    }
                } else {
                    this.attempts.RemoveAt(index);
                }
            }

            foreach (SecretCode code in this.secretCodes) {
                if (code.Letters.StartsWith(key.ToString())) {
                    var attempt = new Attempt(code);
                    attempt.Try(key);
                    this.attempts.Add(attempt);
                }
            }
        }
    }
}