using System;
using System.Collections;
using System.Collections.Generic;
using EunomiaUnity;
using UnityEngine;
using UnityEngine.UI;

namespace EunomiaUnity.TextToSpeech.UI
{
    public class TextToUIText : TextToSpeech
    {
        [SerializeField]
        private Text text;

        [SerializeField]
        private bool useStoppedText = true;

        [SerializeField]
        private string stoppedText = "<stopped>";

        void Awake()
        {
            if (text == null)
            {
                this.LogMissingRequiredReference(typeof(Text));
                gameObject.SetActive(false);
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