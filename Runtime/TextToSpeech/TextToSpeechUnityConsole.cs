using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity.TextToSpeech
{
    /// <summary>
    ///     Text to Unity Console text Text to Speech interface<br />
    ///     A utility class for testing Text to Speech code without all the noise
    /// </summary>
    public class TextToUnityConsole : TextToSpeech
    {
        [SerializeField] private string logPrefix = "TextToSpeech: ";

        public override void Speak(string text)
        {
            Debug.Log($"{logPrefix}{text}");
        }

        public override void StopSpeaking()
        {
            Debug.Log($"{logPrefix}<stopped>");
        }
    }
}