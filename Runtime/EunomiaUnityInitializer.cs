using UnityEngine;

namespace EunomiaUnity
{
    public class EunomiaUnityInitializer : Initializer
    {
        [SerializeField]
        private EunomiaUnity.Random random;
        protected override void Perform()
        {
            random = new EunomiaUnity.Random(UnityEngine.Random.state);
        }
    }
}
