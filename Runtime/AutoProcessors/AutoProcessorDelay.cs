using EunomiaUnity;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    // TODO: this should be built into AutoProcessor
    public class AutoProcessorDelay : AutoProcessor
    {
        [SerializeField, Tooltip("The duration of the delay in seconds")]
        private float delayDuration = 1.0f;

        private bool hasStarted = false;
        public override bool HasStarted => hasStarted;

        private bool hasFinished = false;
        public override bool HasFinished => hasFinished;

        protected override void AutoProcess()
        {
            hasStarted = true;
            StartCoroutine(DelayCoroutine());
        }

        private System.Collections.IEnumerator DelayCoroutine()
        {
            yield return new WaitForSeconds(delayDuration);
            hasFinished = true;
        }
    }
}