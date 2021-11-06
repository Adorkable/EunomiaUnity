using Eunomia;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class UnityEngineRandomWrapper : IRandom
    {
        private readonly UnityEngine.Random.State state;

        public UnityEngineRandomWrapper(UnityEngine.Random.State state)
        {
            this.state = state;
        }

        int IRandom.Next()
        {
            var previousState = UnityEngine.Random.state;

            UnityEngine.Random.state = state;
            var result = UnityEngine.Random.Range(0, int.MaxValue);

            UnityEngine.Random.state = previousState;

            return result;
        }

        int IRandom.Next(int maxValue)
        {
            var previousState = UnityEngine.Random.state;

            var result = UnityEngine.Random.Range(0, maxValue);

            UnityEngine.Random.state = previousState;

            return result;
        }

        int IRandom.Next(int minValue, int maxValue)
        {
            var previousState = UnityEngine.Random.state;

            var result = UnityEngine.Random.Range(minValue, maxValue);

            UnityEngine.Random.state = previousState;

            return result;
        }
    }
}