using System;
using System.Threading.Tasks;
using Eunomia;
using UnityEngine;

namespace EunomiaUnity.TextToSpeech
{
    public class TextToSpeechManager : MonoBehaviour
    {
        [SerializeField]
        private TextToSpeech windowsTextToSpeech;

        [SerializeField]
        private TextToSpeech macOSTextToSpeech;

        [Serializable]
        public enum EditorTextToSpeech
        {
            Console,
            UI,
            Platform
        }
        [SerializeField]
        private EditorTextToSpeech editorTextToSpeech = EditorTextToSpeech.Console;

        private TextToUnityConsole textToUnityConsole;

        [SerializeField]
        private UI.TextToUIText textToUIText;

        public bool TextToSpeechEnabled
        {
            get
            {
                return textToSpeechEnabled;
            }
            set
            {
                textToSpeechEnabled = value;
            }
        }
        private bool textToSpeechEnabled = false;

        void Awake()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        if (windowsTextToSpeech == null)
        {
            this.LogMissingRequiredReference(typeof(TextToSpeech), "WindowsTextToSpeech");
        }
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        if (macOSTextToSpeech == null)
        {
            this.LogMissingRequiredReference(typeof(TextToSpeech), "MacOSTextToSpeech");
        }
#endif

#if UNITY_EDITOR
        textToUnityConsole = gameObject.AddComponent<TextToUnityConsole>();
#endif
        }

        private TextToSpeech GetCurrentTextToSpeech()
        {
#if UNITY_EDITOR
        switch (editorTextToSpeech)
        {
            case EditorTextToSpeech.Console:
                return textToUnityConsole;

            case EditorTextToSpeech.UI:
                return textToUIText;
        }
#endif
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
        return windowsTextToSpeech;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        return macOSTextToSpeech;
#endif
        }

        public async Task StopAndSpeak(string speak, float pauseDuration = 0.5f)
        {
            if (!IsValidSpeechText(speak))
            {
                return;
            }

            try
            {
                StopSpeaking();
            }
            catch (Exception exception)
            {
                Debug.LogError("While attempting to stop speaking", this);
                Debug.LogException(exception, this);
            }

            await Task.Delay((int)(pauseDuration * 1000));

            try
            {
                Speak(speak);
            }
            catch (Exception exception)
            {
                Debug.LogError("While attempting to speak", this);
                Debug.LogException(exception, this);
            }
        }

        protected bool IsValidSpeechText(string text)
        {
            return !String.IsNullOrWhiteSpace(text);
        }

        public void Speak(string text)
        {
            if (textToSpeechEnabled == false)
            {
                return;
            }

            if (!IsValidSpeechText(text))
            {
                return;
            }

            var textToSpeech = GetCurrentTextToSpeech();
            if (textToSpeech == null)
            {
                return;
            }

            textToSpeech.Speak(text);
        }

        public void StopSpeaking()
        {
            if (textToSpeechEnabled == false)
            {
                return;
            }

            var textToSpeech = GetCurrentTextToSpeech();
            if (textToSpeech == null)
            {
                return;
            }

            textToSpeech.StopSpeaking();
        }

        public void SetTextToSpeech(bool enabled)
        {
            textToSpeechEnabled = enabled;
            var textToSpeech = GetCurrentTextToSpeech();
            if (textToSpeech != null)
            {
                textToSpeech.enabled = enabled;
            }
        }

        public void SetTextToSpeechEnabled()
        {
            SetTextToSpeech(true);
        }

        public void SetTextToSpeechDisabled()
        {
            SetTextToSpeech(false);
        }
    }

}
