using System.Linq;
using UnityEngine;
using UnityEngine.TestTools;

namespace EunomiaUnity {
    public class InputTester : MonoBehaviour, IMonoBehaviourTest {
        protected InputMock inputMock = new InputMock();

        public virtual bool IsTestFinished {
            get {
                return this.inputMock.actions.Count == 0;
            }
        }

        public virtual void Update() {
            this.inputMock.Update();
        }
    }
}