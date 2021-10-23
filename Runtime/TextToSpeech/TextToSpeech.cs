using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EunomiaUnity.TextToSpeech
{
    /// <summary>
    /// Text to Speech abstract class
    /// Note: for Foro our target is Jaws so this interface replicates what Jaws makes available and nothing more
    /// </summary>
    public abstract class TextToSpeech : MonoBehaviour
    {
        public abstract void Speak(string text);
        public abstract void StopSpeaking();
    }
}