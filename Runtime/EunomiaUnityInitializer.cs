using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class EunomiaUnityInitializer : Initializer
    {
        [SerializeField] private Random random;
        public Random Random => random;

        protected override void Perform()
        {
            var unityRandom = new UnityEngineRandomWrapper(UnityEngine.Random.state);
            random = new Random(unityRandom);
        }
    }
}