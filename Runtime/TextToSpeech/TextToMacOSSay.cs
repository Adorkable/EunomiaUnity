// based on https://forum.unity.com/threads/easy-speech-synthesis-on-a-mac.524216/

using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity.TextToSpeech
{
    /// <summary>
    ///     macOS Text to Speech interface<br />
    ///     Note: Goes without saying that this is only supported on macOS 😚
    /// </summary>
    public class TextToMacOSSay : TextToSpeech
    {
        [Serializable]
        public enum Voice
        {
            Agnes, //en_US    # Isn't it nice to have a computer that will talk to you?
            Albert, //en_US    #  I have a frog in my throat. No, I mean a real frog!
            Alex, // en_US    # Most people recognize me by my voice.
            Alice, //it_IT    # Salve, mi chiamo Alice e sono una voce italiana.
            Alva, // sv_SE    # Hej, jag heter Alva. Jag är en svensk röst.
            Amelie, //fr_CA    # Bonjour, je m’appelle Amelie. Je suis une voix canadienne.
            Anna, // de_DE    # Hallo, ich heiße Anna und ich bin eine deutsche Stimme.
            BadNews, //en_US    # The light you see at the end of the tunnel is the headlamp of a fast approaching train.
            Bahh, // en_US    # Do not pull the wool over my eyes.
            Bells, //en_US    # Time flies when you are having fun.
            Boing, //en_US    # Spring has sprung, fall has fell, winter's here and it's colder than usual.
            Bruce, //en_US    # I sure like being inside this fancy computer
            Bubbles, //en_US    # Pull the plug! I'm drowning!
            Carmit, //he_IL    # שלום. קוראים לי כרמית, ואני קול בשפה העברית.
            Cellos, //en_US    # Doo da doo da dum dee dee doodly doo dum dum dum doo da doo da doo da doo da doo da doo da doo
            Damayanti, //id_ID    # Halo, nama saya Damayanti. Saya berbahasa Indonesia.
            Daniel, //en_GB    # Hello, my name is Daniel. I am a British-English voice.
            Deranged, //en_US    # I need to go on a really long vacation.
            Diego, //es_AR    # Hola, me llamo Diego y soy una voz española.
            Ellen, //nl_BE    # Hallo, mijn naam is Ellen. Ik ben een Belgische stem.
            Fiona, //en-scotland # Hello, my name is Fiona. I am a Scottish-English voice.
            Fred, // en_US    # I sure like being inside this fancy computer
            GoodNews, //en_US    # Congratulations you just won the sweepstakes and you don't have to pay income tax again.
            Hysterical, //en_US    # Please stop tickling me!
            Ioana, //ro_RO    # Bună, mă cheamă Ioana . Sunt o voce românească.
            Joana, //pt_PT    # Olá, chamo-me Joana e dou voz ao português falado em Portugal.
            Junior, //en_US    # My favorite food is pizza.
            Kanya, //th_TH    # สวัสดีค่ะ ดิฉันชื่อKanya
            Karen, //en_AU    # Hello, my name is Karen. I am an Australian-English voice.
            Kathy, //en_US    # Isn't it nice to have a computer that will talk to you?
            Kyoko, //ja_JP    # こんにちは、私の名前はKyokoです。日本語の音声をお届けします。
            Laura, //sk_SK    # Ahoj. Volám sa Laura . Som hlas v slovenskom jazyku.
            Lekha, //hi_IN    # नमस्कार, मेरा नाम लेखा है.Lekha[[FEMALE_NAME]]मै हिंदी मे बोलने वाली आवाज़ हूँ.
            Luciana, //pt_BR    # Olá, o meu nome é Luciana e a minha voz corresponde ao português que é falado no Brasil
            Mariska, //hu_HU    # Üdvözlöm! Mariska vagyok. Én vagyok a magyar hang.
            Mei_Jia, //zh_TW    # 您好，我叫美佳。我說國語。
            Melina, //el_GR    # Γεια σας, ονομάζομαι Melina. Είμαι μια ελληνική φωνή.
            Milena, //ru_RU    # Здравствуйте, меня зовут Milena. Я – русский голос системы.
            Moira, //en_IE    # Hello, my name is Moira. I am an Irish-English voice.
            Monica, //es_ES    # Hola, me llamo Monica y soy una voz española.
            Nora, // nb_NO    # Hei, jeg heter Nora. Jeg er en norsk stemme.
            Paulina, //es_MX    # Hola, me llamo Paulina y soy una voz mexicana.
            PipeOrgan, //en_US    # We must rejoice in this morbid voice.
            Princess, //en_US    # When I grow up I'm going to be a scientist.
            Ralph, //en_US    # The sum of the squares of the legs of a right triangle is equal to the square of the hypotenuse.
            Samantha, //en_US    # Hello, my name is Samantha. I am an American-English voice.
            Sara, // da_DK    # Hej, jeg hedder Sara. Jeg er en dansk stemme.
            Satu, // fi_FI    # Hei, minun nimeni on Satu. Olen suomalainen ääni.
            Sin_ji, //zh_HK    # 您好，我叫 Sin-ji。我講廣東話。
            Tarik, //ar_SA    # مرحبًا اسمي Tarik. أنا عربي من السعودية.
            Tessa, //en_ZA    # Hello, my name is Tessa. I am a South African-English voice.
            Thomas, //fr_FR    # Bonjour, je m’appelle Thomas. Je suis une voix française.
            Ting_Ting, //zh_CN    # 您好，我叫Ting-Ting。我讲中文普通话。
            Trinoids, //en_US    # We cannot communicate with these carbon units.
            Veena, //en_IN    # Hello, my name is Veena. I am an Indian-English voice.
            Vicki, //en_US    # Isn't it nice to have a computer that will talk to you?
            Victoria, //en_US    # Isn't it nice to have a computer that will talk to you?
            Whisper, //en_US    # Pssssst, hey you, Yeah you, Who do ya think I'm talking to, the mouse?
            Xander, //nl_NL    # Hallo, mijn naam is Xander. Ik ben een Nederlandse stem.
            Yelda, //tr_TR    # Merhaba, benim adım Yelda. Ben Türkçe bir sesim.
            Yuna, // ko_KR    # 안녕하세요. 제 이름은 Yuna입니다. 저는 한국어 음성입니다.
            Zarvox, //en_US    # That looks like a peaceful planet.
            Zosia, //pl_PL    # Witaj. Mam na imię Zosia, jestem głosem kobiecym dla języka polskiego.
            Zuzana //cs_CZ    # Dobrý den, jmenuji se Zuzana. Jsem český hlas.
        }
#if !(UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
#pragma warning disable CS0414 // Unused warning on !OSX
#endif
        [SerializeField] private Voice voice = Voice.Samantha;
        [SerializeField] private int outputChannel = 48;
#if !(UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX)
#pragma warning restore CS0414
#endif

        public UnityEvent OnStartedSpeaking;
        public UnityEvent OnStoppedSpeaking;

        private Process process;
        private bool wasSpeaking;

        public override void Speak(string text)
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX

            var cmdArgs = string.Format("-a {2} -v {0} \"{1}\"", VoiceToString(voice), text.Replace("\"", ","),
                outputChannel);
            process = Process.Start("/usr/bin/say", cmdArgs);
#endif
        }

        public override void StopSpeaking()
        {
#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
            if (process == null)
            {
                return;
            }

            process.Kill();
            process = null;
#endif
        }

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
        private void Update()
        {
            var isSpeaking = process != null && !process.HasExited;
            if (isSpeaking != wasSpeaking)
            {
                if (isSpeaking)
                {
                    OnStartedSpeaking.Invoke();
                }
                else
                {
                    OnStoppedSpeaking.Invoke();
                }

                wasSpeaking = isSpeaking;
            }
        }
#endif

        private static string VoiceToString(Voice voice)
        {
            switch (voice)
            {
                case Voice.BadNews:
                    return "Bad News";
                case Voice.GoodNews:
                    return "Good News";
                case Voice.Mei_Jia:
                    return "Mei-Jia";
                case Voice.PipeOrgan:
                    return "Pipe Organ";
                case Voice.Sin_ji:
                    return "Sin-ji";
                case Voice.Ting_Ting:
                    return "Ting-Ting";

                default:
                    return voice.ToString();
            }
        }
    }
}