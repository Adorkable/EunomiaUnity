using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

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
                    throw new System.Exception(webRequest.error);
                }
                else if (webRequest.isHttpError)
                {
                    throw new System.Exception(webRequest.error);
                }
                else
                {
                    return DownloadHandlerTexture.GetContent(webRequest);
                }
            }
        }

        public static UniTask<Texture2D> LoadUrl(string url)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(url));
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }

        public static UniTask<Texture2D> LoadUrl(Uri uri)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(uri));
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }

        public static UniTask<Texture2D> LoadUrl(string url, bool nonReadable)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(url, nonReadable));
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }
        public static UniTask<Texture2D> LoadUrl(Uri uri, bool nonReadable)
        {
            try
            {
                return Load(UnityWebRequestTexture.GetTexture(uri, nonReadable));
            }
            catch (System.Exception exception)
            {
                throw new System.Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }
    }
}