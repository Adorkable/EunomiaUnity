using System.Collections.Generic;
using System.Linq;
using Eunomia;
using EunomiaUnity;
using NaughtyAttributes;
using UnityEngine;

namespace EunomiaUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceSpectrumDataSource : SpectrumDataSource
    {
        private AudioSource audioSource;
        [SerializeField]
        private FFTWindow spectrumDataWindow = FFTWindow.Rectangular;

        private List<int> sampleRangeValues
        {
            get
            {
                return new int[] {
                64,
                128,
                256,
                512,
                1024,
                2048,
                4096,
                8192
            }.ToList();
            }
        }
        [SerializeField, Dropdown("sampleRangeValues")]
        int sampleRange = 64;
        [SerializeField, Tooltip("Value < 1 to summate all channels")]
        private int channel = -1;

        void Awake()
        {
            audioSource = this.RequireComponentInstance<AudioSource>();
        }

        public override float[] SpectrumData()
        {
            if (audioSource == null)
            {
                return new float[0];
            }
            return audioSource.GetSpectrumDataSafe(sampleRange, channel, spectrumDataWindow);
        }
    }
}