using NaughtyAttributes;
using UnityEngine;

public class AudioClipLoudnessSource : LoudnessSource
{
    [SerializeField]
    private AudioSource audioSource;
    [SerializeField]
    int sampleRange = 100;
    [SerializeField]
    int channel = -1;

    [SerializeField, ReadOnly]
    float loudness;

    public override float Loudness()
    {
        if (audioSource == null)
        {
            return 0;
        }
        return audioSource.CurrentAudioClipLoudness(sampleRange, channel);
    }
}
