using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class Texture2DUtility
    {
        public static async UniTask<Texture2D> Load(UnityWebRequest webRequest)
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

                return DownloadHandlerTexture.GetContent(webRequest);
            }
        }

        public static UniTask<Texture2D> LoadUrl(string url)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(url));
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }

        public static UniTask<Texture2D> LoadUrl(Uri uri)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(uri));
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }

        public static UniTask<Texture2D> LoadUrl(string url, bool nonReadable)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(url, nonReadable));
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }

        public static UniTask<Texture2D> LoadUrl(Uri uri, bool nonReadable)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(uri, nonReadable));
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }
    }
}