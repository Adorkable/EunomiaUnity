using System.Linq;
using UnityEngine;

namespace EunomiaUnity
{

    abstract public class OnFinishedResponder : MonoBehaviour
    {
        // TODO: support any components finished
        [Tooltip("All Components that must be finished")]
        public Component[] WaitToFinish;

        void Update()
        {
            bool allFinished = WaitToFinish.Aggregate(true, (accumulated, component) =>
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

        bool IsFinished(Component component)
        {
            switch (component)
            {
                case OnFinished onFinished:
                    return this.IsFinished(onFinished);

                case ParticleSystem particleSystem:
                    return this.IsFinished(particleSystem);

                case AudioSource audioSource:
                    return this.IsFinished(audioSource);

                default:
                    Debug.LogWarning("Unhandled component type: " + component, this);
                    return true;
            }
        }

        bool IsFinished(OnFinished onFinished)
        {
            return onFinished.IsFinished();
        }

        bool IsFinished(ParticleSystem particleSystem)
        {
            return particleSystem.isStopped;
        }

        bool IsFinished(AudioSource audioSource)
        {
            return !audioSource.isPlaying;
        }

        abstract protected void DoResponse();
    }
}
