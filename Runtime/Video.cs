using Eunomia;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [AddComponentMenu("Video/Video")]
    [RequireComponent(typeof(Renderer))]
    public class Video : VideoBase
    {
        [SerializeField, ShowIf("VideoPlayerIsRenderModeMaterialOverride")]
        public float fadeInDuration = 0.5f;

        [SerializeField, ShowIf("VideoPlayerIsRenderModeMaterialOverride")]
        public float fadeOutDuration = 0.5f;

        [SerializeField, Dropdown("GetFadeMaterialParameterOptions"), ShowIf("VideoPlayerIsRenderModeMaterialOverride")]
        private string fadeMaterialParameter;

        private bool VideoPlayerIsRenderModeMaterialOverride
        {
            get
            {
                return RenderMode == VideoRenderMode.MaterialOverride;
            }
        }

        protected override void Awake()
        {
            base.Awake();

            if (enabled == false)
            {
                return;
            }

            var rendererComponent = this.RequireComponentInstance<Renderer>();

            if (rendererComponent == null)
            {
                enabled = false;
                return;
            }
            else
            {
                rendererComponent.enabled = true;
            }
        }

        protected override void Update()
        {
            base.Update();
            UpdateFade(); // TODO: elaborate insight in Notifications to be able to use it for fading in and fading out
        }

        protected override void OnEnable()
        {
            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = true;
            }
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = false;
            }
        }

        private DropdownList<string> GetFadeMaterialParameterOptions()
        {
            var rendererComponent = this.GetComponent<Renderer>();
            if (rendererComponent == null)
            {
                return null;
            }

            var result = new DropdownList<string>();

            rendererComponent.GetSharedMaterialShaderProperties(ShaderPropertyType.Color)
                .ForEach((parameterName) => result.Add(parameterName, parameterName));

            return result;
        }

        public override void Play()
        {
            if (IsAssignedContent)
            {
                var rendererComponent = gameObject.GetComponent<Renderer>();
                if (rendererComponent != null)
                {
                    rendererComponent.enabled = true;
                }
            }

            base.Play();
        }

        public override void Pause()
        {
            base.Pause();

            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = false;
            }
        }

        public override void Stop()
        {
            base.Stop();

            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = false;
            }
        }

        private void UpdateFade()
        {
            if (fadeInDuration == 0 && fadeOutDuration == 0)
            {
                return;
            }

            if (!VideoPlayerIsRenderModeMaterialOverride)
            {
                return;
            }

            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent == null || rendererComponent.material == null)
            {
                return; // TODO: throw?
            }

            var color = rendererComponent.material.GetColor(fadeMaterialParameter);

            float newAlpha;
            if (!IsPlaying)
            {
                newAlpha = 0;
            }
            else if (fadeInDuration != 0 && Time <= fadeInDuration)
            {
                newAlpha = ((float)Time).Map(0, fadeInDuration, 0, 1);
            }
            else if (fadeOutDuration != 0 && Duration - Time <= fadeOutDuration)
            {
                newAlpha = ((float)(Duration - Time)).Map(0, fadeOutDuration, 0, 1);
            }
            else
            {
                newAlpha = 1;
            }

            // ReSharper disable once CompareOfFloatsByEqualityOperator
            if (newAlpha == color.a)
            {
                return;
            }

            var newColor = new Color(
                color.r,
                color.g,
                color.b,
                newAlpha
            );
            rendererComponent.material.SetColor(fadeMaterialParameter, newColor);
        }

        [Button]
        private void SaveCurrentFrameToDisk()
        {
            var texture = CurrentRenderTexture();
            if (texture == null)
            {
                Debug.LogWarning(
                    $"Unable to Save Current Frame To Disk, unable to retrieve Video Player's Texture to be {typeof(RenderTexture)}, received '{texture}'",
                    this);
                return;
            }

            var savePath =
                $"{Application.temporaryCachePath}/Video_{gameObject.name}-{gameObject.GetInstanceID()}_frame-{Frame}.png";
            texture.WritePNGToDisk(savePath);
            Debug.Log($"Current Frame saved to '{savePath}'", this);
        }
    }
}