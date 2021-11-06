using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public abstract class Processor : MonoBehaviour
    {
        public abstract bool HasStarted { get; }

        public abstract bool HasFinished { get; }
    }
}