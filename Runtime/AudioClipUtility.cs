using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class AudioClipUtility
    {
        public static async UniTask<AudioClip> Load(UnityWebRequest webRequest)
        {
            using (webRequest)
            {
                await webRequest.SendWebRequest();

                if (webRequest.isNetworkError)
                {
                    throw new Exception(webRequest.error);
                }

                if (webRequest.isHttpError)
                {
                    throw new Exception(webRequest.error);
                }

                return DownloadHandlerAudioClip.GetContent(webRequest);
            }
        }

        public static async UniTask<AudioClip> LoadUrl(string url, AudioType audioType)
        {
            try
            {
                var result = await Load(UnityWebRequestMultimedia.GetAudioClip(url, audioType));
                result.name = url;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{url}' of type '{audioType}'): {exception.Message}");
            }
        }

        public static async UniTask<AudioClip> LoadUrl(Uri uri, AudioType audioType)
        {
            try
            {
                var result = await Load(UnityWebRequestMultimedia.GetAudioClip(uri, audioType));
                result.name = uri.OriginalString;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{uri}' of type '{audioType}'): {exception.Message}");
            }
        }

        public static async UniTask<AudioClip> LoadUrl(string url)
        {
            try
            {
                var audioType = AudioTypeForUrl(url);

                var result = await Load(UnityWebRequestMultimedia.GetAudioClip(url, audioType));
                result.name = url;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }

        public static async UniTask<AudioClip> LoadUrl(Uri uri)
        {
            try
            {
                var audioType = AudioTypeForUrl(uri);

                var result = await Load(UnityWebRequestMultimedia.GetAudioClip(uri, audioType));
                result.name = uri.OriginalString;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }

        public static AudioType AudioTypeForUrl(string url)
        {
            var urlLower = url.ToLower();
            if (urlLower.EndsWith(".acc"))
            {
                return AudioType.ACC;
            }

            if (urlLower.EndsWith(".aif") || urlLower.EndsWith(".aiff"))
            {
                return AudioType.AIFF;
            }

            if (urlLower.EndsWith(".mod"))
            {
                return AudioType.MOD;
            }

            if (urlLower.EndsWith(".mpeg") || urlLower.EndsWith(".mpg") || urlLower.EndsWith(".mp2") || urlLower.EndsWith(".mp3"))
            {
                return AudioType.MPEG;
            }

            if (urlLower.EndsWith(".ogg"))
            {
                return AudioType.OGGVORBIS;
            }

            if (urlLower.EndsWith(".s3m"))
            {
                return AudioType.S3M;
            }

            if (urlLower.EndsWith(".wav"))
            {
                return AudioType.WAV;
            }

            if (urlLower.EndsWith(".xm"))
            {
                return AudioType.XM;
            }

            if (urlLower.EndsWith(".xma"))
            {
                return AudioType.XMA;
            }

            if (urlLower.EndsWith(".vag"))
            {
                return AudioType.VAG;
            }

            return AudioType.UNKNOWN;
        }

        public static AudioType AudioTypeForUrl(Uri uri)
        {
            return AudioTypeForUrl(uri.OriginalString);
        }
    }
}