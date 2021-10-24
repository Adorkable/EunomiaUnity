using System;
using System.Threading.Tasks;
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
        public enum EditorTextToSpeechType
        {
            Console,
            UI,
            Platform
        }
        [SerializeField]
        private EditorTextToSpeechType editorTextToSpeech = EditorTextToSpeechType.Console;
        protected EditorTextToSpeechType EditorTextToSpeech => editorTextToSpeech;

        private TextToUnityConsole textToUnityConsole;
        protected TextToUnityConsole TextToUnityConsole => textToUnityConsole;

        [SerializeField]
        private UI.TextToUIText textToUIText;
        protected UI.TextToUIText TextToUIText => textToUIText;

        protected virtual void Awake()
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
            // TODO: only do if selected as current Text To Speech
            textToUnityConsole = gameObject.AddComponent<TextToUnityConsole>();
#endif
        }

        protected TextToSpeech GetCurrentTextToSpeech()
        {
#if UNITY_EDITOR
            switch (editorTextToSpeech)
            {
                case EditorTextToSpeechType.Console:
                    return textToUnityConsole;

                case EditorTextToSpeechType.UI:
                    return textToUIText;
            }
#endif
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return windowsTextToSpeech;
#elif UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            return macOSTextToSpeech;
#endif
        }

        protected virtual void UpdateCurrentTextToSpeech(bool enabled)
        {
            var textToSpeech = GetCurrentTextToSpeech();
            if (textToSpeech != null)
            {
                textToSpeech.enabled = enabled;
            }
        }

        public virtual void OnEnable()
        {
            UpdateCurrentTextToSpeech(true);
        }

        public virtual void OnDisable()
        {
            UpdateCurrentTextToSpeech(false);
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
            if (enabled == false)
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
            if (enabled == false)
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
    }

}
