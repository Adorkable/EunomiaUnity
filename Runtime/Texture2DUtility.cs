using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    /// <summary> A collection of Texture2D utilities including loading textures from remotes sources. </summary>
    public static class Texture2DUtility
    {
        /// <summary> Loads a texture from a remote source. </summary>
        /// <param name="webRequest"> The <see cref="UnityWebRequest"/> to load the texture from. </param>
        /// <returns> The loaded texture. </returns>
        /// <exception cref="Exception"> Thrown when a network error condition occurs. </exception>
        /// <exception cref="Exception"> Thrown when a UnityWebRequest error condition occurs. </exception>
        public static async UniTask<Texture2D> Load(UnityWebRequest webRequest)
        {
            using (webRequest)
            {
                await webRequest.SendWebRequestThrowing();

                return DownloadHandlerTexture.GetContent(webRequest);
            }
        }

        /// <summary> Loads a texture from a remote source. </summary>
        /// <param name="url"> The URL to load the texture from. </param>
        /// <returns> The loaded texture. </returns>
        /// <exception cref="Exception"> Thrown when a network error condition occurs. </exception>
        /// <exception cref="Exception"> Thrown when a UnityWebRequest error condition occurs. </exception>
        public static async UniTask<Texture2D> LoadUrl(string url)
        {
            try
            {
                var result = await Load(UnityWebRequestTexture.GetTexture(url));
                result.name = url;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }


        /// <summary> Loads a texture from a remote source. </summary>
        /// <param name="uri"> The URI to load the texture from. </param>
        /// <returns> The loaded texture. </returns>
        /// <exception cref="Exception"> Thrown when a network error condition occurs. </exception>
        /// <exception cref="Exception"> Thrown when a UnityWebRequest error condition occurs. </exception>
        public static async UniTask<Texture2D> LoadUrl(Uri uri)
        {
            try
            {
                var result = await Load(UnityWebRequestTexture.GetTexture(uri));
                result.name = uri.OriginalString;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }

        /// <summary> Loads a texture from a remote source. </summary>
        /// <param name="url"> The URL to load the texture from. </param>
        /// <param name="nonReadable"> true if the texture should be non-readable. </param>
        /// <returns> The loaded texture. </returns>
        /// <exception cref="Exception"> Thrown when a network error condition occurs. </exception>
        /// <exception cref="Exception"> Thrown when a UnityWebRequest error condition occurs. </exception>
        public static async UniTask<Texture2D> LoadUrl(string url, bool nonReadable)
        {
            try
            {
                var result = await Load(UnityWebRequestTexture.GetTexture(url, nonReadable));
                result.name = url;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{url}'): {exception.Message}");
            }
        }

        /// <summary> Loads a texture from a remote source. </summary>
        /// <param name="uri"> The URI to load the texture from. </param>
        /// <param name="nonReadable"> true if the texture should be non-readable. </param>
        /// <returns> The loaded texture. </returns>
        /// <exception cref="Exception"> Thrown when a network error condition occurs. </exception>
        /// <exception cref="Exception"> Thrown when a UnityWebRequest error condition occurs. </exception>
        public static async UniTask<Texture2D> LoadUrl(Uri uri, bool nonReadable)
        {
            try
            {
                var result = await Load(UnityWebRequestTexture.GetTexture(uri, nonReadable));
                result.name = uri.OriginalString;
                return result;
            }
            catch (Exception exception)
            {
                throw new Exception($"Network error for request '{uri}'): {exception.Message}");
            }
        }
    }
}