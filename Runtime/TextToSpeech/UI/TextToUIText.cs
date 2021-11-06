using UnityEngine;
using UnityEngine.UI;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity.TextToSpeech.UI
{
    public class TextToUIText : TextToSpeech
    {
        [SerializeField] private Text text;

        [SerializeField] private bool useStoppedText = true;

        [SerializeField] private string stoppedText = "<stopped>";

        private void Awake()
        {
            if (text == null)
            {
                this.LogMissingRequiredReference(typeof(Text));
                enabled = false;
            }
        }

        public override void Speak(string speak)
        {
            if (text == null)
            {
                return;
            }

            text.text = speak;
        }

        public override void StopSpeaking()
        {
            if (!useStoppedText)
            {
                return;
            }

            if (text == null)
            {
                return;
            }

            text.text = stoppedText;
        }
    }
}