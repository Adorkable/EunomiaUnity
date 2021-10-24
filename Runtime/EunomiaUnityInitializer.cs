using UnityEngine;

namespace EunomiaUnity
{
    public class EunomiaUnityInitializer : Initializer
    {
        [SerializeField]
        private EunomiaUnity.Random random;
        public EunomiaUnity.Random Random => random;

        protected override void Perform()
        {
            var unityRandom = new UnityEngineRandomWrapper(UnityEngine.Random.state);
            random = new EunomiaUnity.Random(unityRandom);
        }
    }
}
