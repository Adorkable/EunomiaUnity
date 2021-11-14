using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity.UI
{
    [AddComponentMenu("UI/Image (Eunomia)")]
    [RequireComponent(typeof(UnityEngine.UI.Image))]
    public class Image : MonoBehaviour
    {
        [ShowAssetPreview] private UnityEngine.UI.Image image;

        private string url;

        [ShowNativeProperty]
        public string Url
        {
            get { return url; }
            set { _ = Set(value); }
        }

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

        private void Awake()
        {
            image = this.RequireComponentInstance<UnityEngine.UI.Image>();
            if (image == null)
            {
                enabled = false;
            }
        }

        private void OnEnable()
        {
            if (image != null)
            {
                image.enabled = true;
            }
        }

        private void OnDisable()
        {
            if (image != null)
            {
                image.enabled = false;
            }
        }

        public async UniTask Set(string newUrl)
        {
            url = newUrl;

            if (image != null)
            {
                image.sprite = await SpriteUtility.LoadUrl(newUrl);
            }
        }
    }
}