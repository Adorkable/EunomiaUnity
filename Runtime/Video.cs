using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eunomia;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Video;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [RequireComponent(typeof(VideoPlayer)), RequireComponent(typeof(Renderer))]
    public class Video : MonoBehaviour
    {
        [SerializeField, ShowIf("VideoPlayerIsRenderModeMaterialOverride")]
        public float fadeInDuration = 0.5f;

        [SerializeField, ShowIf("VideoPlayerIsRenderModeMaterialOverride")]
        public float fadeOutDuration = 0.5f;

        [SerializeField, Dropdown("GetFadeMaterialParameterOptions"), ShowIf("VideoPlayerIsRenderModeMaterialOverride")]
        private string fadeMaterialParameter;

        private readonly List<UniTaskCompletionSource<Video>> onPrepared = new List<UniTaskCompletionSource<Video>>();

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute - not obvious to me on first glance
        [SerializeField, Foldout("Debug"), AllowNesting, ReadOnly]
        private List<INotification> notifications;

        private VideoPlayer videoPlayer;

        private bool VideoPlayerIsRenderModeMaterialOverride
        {
            get
            {
                if (videoPlayer == null)
                {
                    return false;
                }

                return videoPlayer.renderMode == VideoRenderMode.MaterialOverride;
            }
        }

        [ShowNativeProperty]
        public bool IsLooping
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.isLooping;
                }

                return false;
            }
            set { videoPlayer.isLooping = value; }
        }

        [ShowNativeProperty]
        public bool IsPlaying
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.isPlaying;
                }

                return false;
            }
        }

        [ShowNativeProperty]
        public VideoClip VideoClip
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.clip;
                }

                return null;
            }
            set { _ = SetVideoClip(value); }
        }

        [ShowNativeProperty]
        public string Url
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.url;
                }

                return null;
            }
            set { _ = SetUrl(value); }
        }

        [ShowNativeProperty]
        public double Duration
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.length;
                }

                return 0;
            }
        }

        [ShowNativeProperty]
        public double Time
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.time;
                }

                return 0;
            }
            set
            {
                if (videoPlayer != null)
                {
                    videoPlayer.time = value;
                }
            }
        }

        [ShowNativeProperty]
        public Vector2Int Size
        {
            get
            {
                if (videoPlayer == null || videoPlayer.texture == null)
                {
                    return new Vector2Int(0, 0);
                }

                var texture = videoPlayer.texture;
                return new Vector2Int(texture.width, texture.height);
            }
        }

        [ShowNativeProperty]
        public bool PlayOnAwake
        {
            get
            {
                if (videoPlayer != null)
                {
                    return videoPlayer.playOnAwake;
                }

                return false;
            }
            set
            {
                if (videoPlayer != null)
                {
                    videoPlayer.playOnAwake = value;
                }
            }
        }

        public bool IsAssignedContent
        {
            get
            {
                if (videoPlayer != null)
                {
                    return !String.IsNullOrWhiteSpace(videoPlayer.url) || videoPlayer.clip != null;
                }

                return false;
            }
        }

        protected void Awake()
        {
            videoPlayer = this.RequireComponentInstance<VideoPlayer>();
            var rendererComponent = this.RequireComponentInstance<Renderer>();

            if (videoPlayer == null)
            {
                enabled = false;
                return;
            }
            else
            {
                videoPlayer.enabled = true;
            }

            if (rendererComponent == null)
            {
                enabled = false;
                return;
            }
            else
            {
                rendererComponent.enabled = true;
            }

            videoPlayer.prepareCompleted += VideoPlayerPrepareCompleted;
            videoPlayer.started += VideoPlayerStarted;
            videoPlayer.loopPointReached += VideoPlayerLoopPointReached;
            videoPlayer.errorReceived += VideoPlayerErrorRecieved;

            notifications = new List<INotification>();
        }

        private void Update()
        {
            UpdateNotifications();
            UpdateFade(); // TODO: elaborate insight in Notifications to be able to use it for fading in and fading out
        }

        private void OnEnable()
        {
            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = true;
            }

            Play();
        }

        private void OnDisable()
        {
            Pause();
            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = false;
            }
        }

        private void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPrepareCompleted;
            videoPlayer.started -= VideoPlayerStarted;
            videoPlayer.loopPointReached -= VideoPlayerLoopPointReached;
            videoPlayer.errorReceived -= VideoPlayerErrorRecieved;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            videoPlayer = this.RequireComponentInstance<VideoPlayer>();
#endif
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

        public event EventHandler OnPrepared;
        public event EventHandler OnStarted;
        public event EventHandler OnLoopPointReached;

        private void VideoPlayerPrepareCompleted(VideoPlayer source)
        {
            IList<UniTaskCompletionSource<Video>> taskCompletionSources;
            lock (onPrepared)
            {
                taskCompletionSources = onPrepared.Copy();
                onPrepared.Clear();
            }

            var instance = this;
            taskCompletionSources.ForEach(
                (taskCompletionSource, index) => taskCompletionSource.TrySetResult(instance)
            );
            OnPrepared?.Invoke(this, EventArgs.Empty);
        }

        private void VideoPlayerStarted(VideoPlayer source)
        {
            OnStarted?.Invoke(this, EventArgs.Empty);
        }

        private void VideoPlayerLoopPointReached(VideoPlayer source)
        {
            OnLoopPointReached?.Invoke(this, EventArgs.Empty);
        }

        private void VideoPlayerErrorRecieved(VideoPlayer source, string error)
        {
            Debug.LogError(error, this);
            VideoPlayerLoopPointReached(source);
        }

        public void Prepare()
        {
            if (IsAssignedContent)
            {
                videoPlayer.Prepare();
            }
        }

        public void Play()
        {
            if (IsAssignedContent)
            {
                var rendererComponent = gameObject.GetComponent<Renderer>();
                if (rendererComponent != null)
                {
                    rendererComponent.enabled = true;
                }

                videoPlayer.Play();
            }
        }

        public void Pause()
        {
            videoPlayer.Pause();
            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = false;
            }
        }

        public void Stop()
        {
            videoPlayer.Stop();
            var rendererComponent = gameObject.GetComponent<Renderer>();
            if (rendererComponent != null)
            {
                rendererComponent.enabled = false;
            }
        }

        private UniTask PrepareAfterSet()
        {
            videoPlayer.Prepare();

            var prepareTask = new UniTaskCompletionSource<Video>();
            lock (onPrepared)
            {
                onPrepared.Add(prepareTask);
            }

            return prepareTask.Task;
        }

        public UniTask SetVideoClip(VideoClip videoClip)
        {
            videoPlayer.clip = videoClip;
            if (videoClip != null)
            {
                return PrepareAfterSet();
            }
            else
            {
                Stop();
                return UniTask.CompletedTask;
            }
        }

        public UniTask SetUrl(string url)
        {
            videoPlayer.url = url;
            if (String.IsNullOrWhiteSpace(videoPlayer.url) == false)
            {
                return PrepareAfterSet();
            }
            else
            {
                Stop();
                return UniTask.CompletedTask;
            }
        }

        public void AddNotificationAtPercent(Action<Video> perform, float atPercent)
        {
            notifications.Add(new AtPercentNotification()
            {
                Perform = perform,
                atPercent = atPercent
            });
        }

        public void AddNotificationAtTimeFromEnd(Action<Video> perform, float atTimeFromEnd)
        {
            notifications.Add(new AtTimeFromEndNotification()
            {
                Perform = perform,
                atTimeFromEnd = atTimeFromEnd
            });
        }

        private void PerformAndRemove(INotification perform)
        {
            perform.Perform.Invoke(this);
            notifications.Remove(perform);
        }

        private void UpdateNotifications()
        {
            if (videoPlayer.length == 0)
            {
                return;
            }

            var index = 0;
            while (index < notifications.Count)
            {
                var notification = notifications[index];
                switch (notification)
                {
                    case AtPercentNotification atPercent:
                        if (atPercent.atPercent <= videoPlayer.time / videoPlayer.length)
                        {
                            PerformAndRemove(atPercent);
                            continue;
                        }

                        break;
                    case AtTimeFromEndNotification atTimeFromEnd:
                        if (atTimeFromEnd.atTimeFromEnd >= videoPlayer.length - videoPlayer.time)
                        {
                            PerformAndRemove(atTimeFromEnd);
                            continue;
                        }

                        break;
                }

                index++;
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
            if (videoPlayer == null)
            {
                Debug.LogWarning(
                    "Unable to Save Current Frame To Disk, Video Player not found", this);
                return;
            }

            var texture = videoPlayer.CurrentRenderTexture();
            if (texture == null)
            {
                Debug.LogWarning(
                    $"Unable to Save Current Frame To Disk, expected Video Player's Texture to be {typeof(RenderTexture)}, received '{videoPlayer.texture.GetType()}'",
                    this);
                return;
            }

            var savePath =
                $"{Application.temporaryCachePath}/Video_{gameObject.name}-{gameObject.GetInstanceID()}_frame-{videoPlayer.frame}.png";
            texture.WritePNGToDisk(savePath);
            Debug.Log($"Current Frame saved to '{savePath}'", this);
        }

        private interface INotification
        {
            Action<Video> Perform { get; }
        }

        [Serializable]
        private class AtPercentNotification : INotification
        {
            public float atPercent;
            public Action<Video> Perform { get; set; }

            public override string ToString()
            {
                // ReSharper disable once HeapView.BoxingAllocation
                return $"At percent: {atPercent}%";
            }
        }

        [Serializable]
        private class AtTimeFromEndNotification : INotification
        {
            public float atTimeFromEnd;
            public Action<Video> Perform { get; set; }

            public override string ToString()
            {
                // ReSharper disable once HeapView.BoxingAllocation
                return $"At time from end: {atTimeFromEnd} seconds";
            }
        }
    }
}