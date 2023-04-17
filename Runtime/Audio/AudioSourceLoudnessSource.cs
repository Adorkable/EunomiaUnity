using EunomiaUnity;
using UnityEngine;

namespace EunomiaUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceLoudnessSource : LoudnessSource
    {
        private AudioSource audioSource;
        // [SerializeField]
        // int sampleRange = 1; // TODO: use as upper limit for OnAudioFilterRead range
        [SerializeField, Tooltip("Value < 1 to summate all channels")]
        private int channel = -1;

        private float loudness = 0;

        void Awake()
        {
            audioSource = this.RequireComponentInstance<AudioSource>();
        }

        public override float Loudness()
        {
            return loudness;
        }

        public void OnAudioFilterRead(float[] data, int channels)
        {
            var sumLoudness = 0.0f;
            var length = data.Length / channels;
            for (var index = 0; index < length; index++)
            {
                if (channel < 0)
                {
                    for (var channelIndex = 0; channelIndex < channels; channelIndex++)
                    {
                        sumLoudness += Mathf.Abs(data[index * channels + channelIndex]);
                    }
                }
                else
                {
                    sumLoudness += Mathf.Abs(data[index * channels + channel % channels]);
                }
            }
            loudness = sumLoudness / length;
        }
    }
}