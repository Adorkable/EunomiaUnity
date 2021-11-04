
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace EunomiaUnity
{
    public static class SpriteUtility
    {
        public static async UniTask<Sprite> LoadUrl(string url)
        {
            var texture2D = await Texture2DUtility.LoadUrl(url);
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f), 100);
        }
    }
}