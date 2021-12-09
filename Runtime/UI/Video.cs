using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity.UI
{
    [AddComponentMenu("UI/Video")]
    [RequireComponent(typeof(RawImage))]
    public class Video : VideoBase
    {
        [SerializeField] private RenderTexture renderTextureTemplate;

        [SerializeField] private Texture2D editorPreview;

        private RawImage rawImage;
        private RenderTexture renderTexture;

        protected override void Awake()
        {
            base.Awake();

            if (enabled == false)
            {
                return;
            }

            rawImage = this.RequireComponentInstance<RawImage>();

            if (rawImage == null)
            {
                enabled = false;
                return;
            }
            else
            {
                rawImage.enabled = true;
            }

            if (renderTextureTemplate == null)
            {
                this.LogMissingRequiredReference(typeof(VideoPlayer));
                enabled = false;
                return;
            }

            // var rawImageTransform = rawImage.GetComponent<RectTransform>();
            renderTexture = new RenderTexture(renderTextureTemplate);
            renderTexture.name = this.name;
            renderTexture.Create();

            RenderMode = VideoRenderMode.RenderTexture;
            TargetTexture = renderTexture;

            rawImage.texture = renderTexture;
        }

        protected override void OnEnable()
        {
            rawImage.enabled = true;
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            rawImage.enabled = false;
            base.OnDisable();
        }

        protected override void OnValidate()
        {
            base.OnValidate();
#if UNITY_EDITOR
            var rawImageComponent = GetComponent<RawImage>();
            if (rawImageComponent != null)
            {
                rawImageComponent.texture = editorPreview;
            }
#endif
        }

        public override void Stop()
        {
            base.Stop();

            if (rawImage != null && rawImage.texture != null && rawImage.texture is RenderTexture renderTexture)
            {
                renderTexture.Clear();
            }
        }

        [Button]
        private void SaveCurrentFrameToDisk()
        {
            if (rawImage == null || rawImage.texture == null)
            {
                Debug.LogWarning(
                    "Raw Image, Raw Image's texture or Video Player not found, unable to Save Current Frame To Disk",
                    this);
                return;
            }

            var rawImageTexture = (RenderTexture)rawImage.texture;
            if (rawImageTexture == null)
            {
                Debug.LogWarning($"Unexpected texture type '{rawImage.texture.GetType()}'", this);
                return;
            }

            var savePath =
                $"{Application.temporaryCachePath}/Video_{gameObject.name}-{gameObject.GetInstanceID()}_frame-{Frame}.png";
            rawImageTexture.WritePNGToDisk(savePath);
            Debug.Log($"Current Frame saved to '{savePath}'", this);
        }

        public void SetRenderTextureSize(Vector2 size)
        {
            var resized = new RenderTexture((int)size.x, (int)size.y, renderTexture.depth, renderTexture.format);
            resized.name = this.name;
            resized.Create();

            TargetTexture = resized;
            rawImage.texture = resized;
            renderTexture = resized;
        }
    }
}