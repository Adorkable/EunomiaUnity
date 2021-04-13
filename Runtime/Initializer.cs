using UnityEngine;

namespace EunomiaUnity
{
    // TODO: combine with OnFinished for On...
    public abstract class Initializer : MonoBehaviour
    {
        public enum When
        {
            Awake,
            Start
        };
        public When initializeWhen = When.Awake;

        private void Awake()
        {
            if (initializeWhen == When.Awake)
            {
                Perform();
            }
        }

        private void Start()
        {
            if (initializeWhen == When.Start)
            {
                Perform();
            }
        }

        protected abstract void Perform();
    }
}
