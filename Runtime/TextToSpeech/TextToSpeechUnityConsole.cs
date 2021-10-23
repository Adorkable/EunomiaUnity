
using System.Diagnostics;
using UnityEngine;

namespace EunomiaUnity.TextToSpeech
{
    /// <summary>
    /// Text to Unity Console text Text to Speech interface<br/>
    /// A utility class for testing Text to Speech code without all the noise
    /// </summary>
    public class TextToUnityConsole : TextToSpeech
    {
        [SerializeField]
        private string logPrefix = "TextToSpeech: ";

        public override void Speak(string text)
        {
            UnityEngine.Debug.Log($"{logPrefix}{text}");
        }

        public override void StopSpeaking()
        {
            UnityEngine.Debug.Log($"{logPrefix}<stopped>");
        }
    }
}