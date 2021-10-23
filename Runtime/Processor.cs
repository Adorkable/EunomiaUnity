using UnityEngine;

public abstract class Processor : MonoBehaviour
{
    public abstract bool HasStarted { get; }

    public abstract bool HasFinished { get; }
}
