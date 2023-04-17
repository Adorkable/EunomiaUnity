using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace EunomiaUnity
{
    [RequireComponent(typeof(AudioSource))]
    public class MicrophoneAudioSource : MonoBehaviour
    {
        private List<string> deviceNames
        {
            get
            {
                return Microphone.devices.ToList();
            }
        }
        [SerializeField, Dropdown("deviceNames")]
        private string microphoneName;

        [SerializeField]
        public AudioMixerGroup audioMixerGroup;

        private AudioClip audioClip;
        private AudioSource audioSource;

        public void Awake()
        {
            audioSource = this.GetComponent<AudioSource>();
            audioSource.loop = true;
            audioSource.outputAudioMixerGroup = audioMixerGroup;
        }

        public void Start()
        {
            Restart();
        }

        public void Reset()
        {
            Restart();
        }

        void Restart()
        {
            Microphone.End(microphoneName);
            audioSource.Stop();

            audioClip = Microphone.Start(microphoneName, true, 5, AudioSettings.outputSampleRate);

            audioSource.clip = audioClip;

            while (!(Microphone.GetPosition(microphoneName) - 0 > 0)) { }
            audioSource.Play();
        }

    }
}