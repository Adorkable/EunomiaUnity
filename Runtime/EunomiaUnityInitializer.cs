namespace EunomiaUnity
{
    public class EunomiaUnityInitializer : Initializer
    {
        private EunomiaUnity.Random random;
        protected override void Perform()
        {
            random = new EunomiaUnity.Random(UnityEngine.Random.state, new Eunomia.WordList());
        }
    }
}
