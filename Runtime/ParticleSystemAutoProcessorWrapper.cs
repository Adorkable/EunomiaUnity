using EunomiaUnity;
using UnityEngine;

public class ParticleSystemAutoProcessorWrapper : AutoProcessor
{
    [SerializeField]
    private new ParticleSystem particleSystem;

    public override bool HasStarted => particleSystem.isPlaying;

    private bool wasStarted = false;

    public override bool HasFinished => wasStarted && particleSystem.isStopped;

    protected override void AutoProcess()
    {
        particleSystem.Play();
        wasStarted = true;
    }
}