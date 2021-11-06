using System.Linq;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    abstract public class OnFinishedResponder : MonoBehaviour
    {
        // TODO: support any components finished
        [Tooltip("All Components that must be finished")]
        public Component[] WaitToFinish;

        private void Update()
        {
            var allFinished = WaitToFinish.Aggregate(true, (accumulated, component) =>
            {
                if (accumulated == false)
                {
                    return false;
                }

                return this.IsFinished(component);
            });
            if (allFinished)
            {
                this.DoResponse();
            }
        }

        private bool IsFinished(Component component)
        {
            switch (component)
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                case IOnFinished onFinished:
                    return this.IsFinished(onFinished);

                case ParticleSystem testParticleSystem:
                    return this.IsFinished(testParticleSystem);

                case AudioSource testAudioSource:
                    return this.IsFinished(testAudioSource);

                default:
                    Debug.LogWarning($"Unhandled component type: {component}", this);
                    return true;
            }
        }

        private bool IsFinished(IOnFinished onFinished)
        {
            return onFinished.IsFinished();
        }

        private bool IsFinished(ParticleSystem test)
        {
            return test.isStopped;
        }

        private bool IsFinished(AudioSource test)
        {
            return !test.isPlaying;
        }

        abstract protected void DoResponse();
    }
}