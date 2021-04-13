using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Tests {
    public class SecretCodeTests {

        // TODO: test multiple secret codes

        private class SecretCodeTester : EunomiaUnity.InputTester {
            protected EunomiaUnity.SecretCodeDetector secretCodeDetector;
            protected SecretCodeTesterMatchEvent matchEvent;

            protected class SecretCodeTesterMatchEvent : EunomiaUnity.SecretCode.MatchEvent {
                public static string MatchedLog(string letters) {
                    return letters + " matched";
                }

                protected bool matched = false;

                public SecretCodeTesterMatchEvent(string letters) {
                    this.AddListener(() => {
                        Debug.Log(MatchedLog(letters));

                        this.matched = true;
                    });
                }

                public bool IsMatched {
                    get {
                        return this.matched;
                    }
                }
            }

            public virtual void Awake() {
                this.secretCodeDetector = this.gameObject.AddComponent<EunomiaUnity.SecretCodeDetector>();
                this.secretCodeDetector.input = this.inputMock;
            }

            protected void AddSecretCode(string letters) {
                this.matchEvent = new SecretCodeTesterMatchEvent(letters);
                this.secretCodeDetector.secretCodes = new EunomiaUnity.SecretCode[] {
                    new EunomiaUnity.SecretCode {
                        Letters = letters,
                        OnMatch = this.matchEvent
                    }
                };
            }

            public bool IsMatched {
                get {
                    return this.matchEvent.IsMatched;
                }
            }
        }


        private class SuccessTester : SecretCodeTester {
            public override void Awake() {
                base.Awake();

                var letters = "SUCCESS";
                this.AddSecretCode(letters);
                LogAssert.Expect(LogType.Log, SecretCodeTesterMatchEvent.MatchedLog(letters));

                List<EunomiaUnity.InputMock.Action> actions = new List<EunomiaUnity.InputMock.Action> {
                    new EunomiaUnity.InputMock.Wait(1),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.S),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.U),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.C),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.C),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.E),
                    new EunomiaUnity.InputMock.Wait(1),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.S),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.S),
                    new EunomiaUnity.InputMock.Wait(1),
                };
                this.inputMock.actions.AddRange(actions);
            }
        }

        [UnityTest]
        public IEnumerator TestSuccess() {
            yield return new MonoBehaviourTest<SuccessTester>();
        }

        private class TimeoutTester : SecretCodeTester {
            public override void Awake() {
                base.Awake();

                var letters = "TIMEOUT";
                this.AddSecretCode(letters);

                List<EunomiaUnity.InputMock.Action> actions = new List<EunomiaUnity.InputMock.Action> {
                    new EunomiaUnity.InputMock.Wait(1),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.T),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.I),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.M),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.E),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.O),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.U),
                    new EunomiaUnity.InputMock.Wait(11),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.T),
                    new EunomiaUnity.InputMock.Wait(1),
                };
                this.inputMock.actions.AddRange(actions);
            }

            public override void Update() {
                base.Update();

                Debug.Log("Updated");
                if (!this.IsMatched) {
                    Assert.Fail("Secret code should not match");
                }
            }
        }

        [UnityTest]
        public IEnumerable TestTimeout() {
            yield return new MonoBehaviourTest<TimeoutTester>();

        }

        private class FailureTester : SecretCodeTester {
            public override void Awake() {
                base.Awake();

                var letters = "FAILURE";
                this.AddSecretCode(letters);

                List<EunomiaUnity.InputMock.Action> actions = new List<EunomiaUnity.InputMock.Action> {
                    new EunomiaUnity.InputMock.Wait(1),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.F),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.A),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.I),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.L),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.U),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.R),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.X),

                    new EunomiaUnity.InputMock.Wait(11),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.F),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.A),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.I),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.L),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.U),
                    new EunomiaUnity.InputMock.KeyDown(KeyCode.R),
                    new EunomiaUnity.InputMock.Wait(1),
                };
                this.inputMock.actions.AddRange(actions);
            }

            public override void Update() {
                base.Update();

                if (this.IsMatched) {
                    Assert.Fail("Secret code should not match");
                }
            }
        }

        [UnityTest]
        public IEnumerable TestFailure() {
            yield return new MonoBehaviourTest<FailureTester>();
        }
    }
}
