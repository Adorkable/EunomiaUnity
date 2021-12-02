using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TestTools;

namespace EunomiaUnity
{
    public class InputMock : EunomiaUnity.IGetKeyDownSource
    {
        public interface Action { }

        public class KeyDown : Action
        {
            public KeyCode keyCode;

            public KeyDown(KeyCode keyCode)
            {
                this.keyCode = keyCode;
            }
        }

        public class Wait : Action
        {
            public float waitTimeInSeconds;

            public Wait(float waitTimeInSeconds)
            {
                this.waitTimeInSeconds = waitTimeInSeconds;
            }
        }

        public List<Action> actions = new List<Action>();

        private float timeSinceLastActionInSeconds = 0;
        private float lastUpdateTime = -1;

        public bool GetKeyDown(KeyCode key)
        {
            if (this.actions.Count == 0)
            {
                return false;
            }

            switch (this.actions[0])
            {
                case KeyDown keyDown:
                    return keyDown.keyCode == key;

                case Wait wait:
                    return false;

                default:
                    return false;
            }
        }

        public void Update()
        {
            if (this.lastUpdateTime != -1)
            {
                this.timeSinceLastActionInSeconds += Time.time - this.lastUpdateTime;
            }
            this.lastUpdateTime = Time.time;

            if (this.actions.Count == 0)
            {
                return;
            }

            bool resetTime = false;
            switch (this.actions[0])
            {
                case KeyDown keyDown:
                    this.actions.RemoveAt(0);
                    resetTime = true;
                    break;

                case Wait wait:
                    if (this.timeSinceLastActionInSeconds >= wait.waitTimeInSeconds)
                    {
                        this.actions.RemoveAt(0);
                        resetTime = true;
                    }
                    break;

                default:
                    break;
            }

            if (resetTime)
            {
                this.timeSinceLastActionInSeconds = 0;
            }
        }
    }

}
