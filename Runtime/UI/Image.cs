using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using EunomiaUnity;
using NaughtyAttributes;
using UnityEngine;

namespace EunomiaUnity.UI
{
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class Image : MonoBehaviour
    {
        [ShowAssetPreview]
        private UnityEngine.UI.Image image;

        [ShowNativeProperty]
        public string Url
        {
            get
            {
                return url;
            }
            set
            {
                _ = Set(value);
            }
        }
        private string url;

        [ShowNativeProperty]
        public Vector2Int Size
        {
            get
            {
                if (image == null)
                {
                    return new Vector2Int(0, 0);
                }

                return new Vector2Int(image.sprite.texture.width, image.sprite.texture.height);
            }
        }

        void Awake()
        {
            image = this.RequireComponentInstance<UnityEngine.UI.Image>();
            if (image == null)
            {
                enabled = false;
            }
        }

        public async UniTask Set(string url)
        {
            this.url = url;

            if (image != null)
            {
                image.sprite = await SpriteUtility.LoadUrl(url);
            }
        }

        void OnEnable()
        {
            if (image != null)
            {
                image.enabled = true;
            }
        }

        void OnDisable()
        {
            if (image != null)
            {
                image.enabled = false;
            }
        }
    }
}