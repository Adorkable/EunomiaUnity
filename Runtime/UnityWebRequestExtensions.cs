using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class UnityWebRequestExtensions
    {
        public static async UniTask<UnityWebRequest> SendWebRequestThrowing(this UnityWebRequest webRequest)
        {
            await webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError)
            {
                throw new Exception(webRequest.error);
            }

            if (webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                throw new Exception(webRequest.error);
            }

            if (webRequest.result == UnityWebRequest.Result.DataProcessingError)
            {
                throw new Exception(webRequest.error);
            }

            return webRequest;
        }
    }
}