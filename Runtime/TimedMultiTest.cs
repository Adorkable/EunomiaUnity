using System.Threading.Tasks;
using Eunomia;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public abstract class TimedMultiTest<TContent> : MonoBehaviour
    {
        public float testDuration = 5;
        private int testIndex;
        private float testStartTime;
        private bool transitioning;
        protected abstract TContent[] Content { get; }

        protected virtual void Awake()
        {
            if (Content.Length == 0)
            {
                Debug.LogError("No test content provided", this);
                enabled = false;
                return;
            }

            SwitchToTest(0);
        }

        private void Update()
        {
            // TODO: only do when visible
            if (testStartTime + testDuration < Time.time)
            {
                SwitchToTest(testIndex + 1);
            }
        }

        private async void SwitchToTest(int index)
        {
            lock (this)
            {
                if (transitioning)
                {
                    return;
                }

                transitioning = true;
            }

            testIndex = index.Wrap(Content.Length);
            testStartTime = Time.time;

            await StartTest(Content[testIndex]);

            lock (this)
            {
                transitioning = false;
            }
        }

        protected abstract Task StartTest(TContent content);
    }
}