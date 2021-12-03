using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eunomia;
using NaughtyAttributes;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

// TODO: rethink if we can merge this and Video/Video

// ReSharper disable once CheckNamespace
namespace EunomiaUnity.UI
{
    [AddComponentMenu("UI/Video")]
    [RequireComponent(typeof(VideoPlayer)), RequireComponent(typeof(RawImage))]
    public class Video : MonoBehaviour, IVideo
    {
        [SerializeField] private RenderTexture renderTextureTemplate;

        [SerializeField] private Texture2D editorPreview;

        private readonly List<UniTaskCompletionSource<Video>> onPrepared = new List<UniTaskCompletionSource<Video>>();

        // ReSharper disable once Unity.RedundantSerializeFieldAttribute - not obvious on first glance
        [SerializeField, Foldout("Debug"), AllowNesting, ReadOnly]
        private List<INotification> notifications;

        private RawImage rawImage;
        private RenderTexture renderTexture;
        private VideoPlayer videoPlayer;

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
                if (videoPlayer == null)
                {
                    return new Vector2Int(0, 0);
                }

                var texture = videoPlayer.texture;
                if (texture == null)
                {
                    return new Vector2Int(0, 0);
                }

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
                    return !String.IsNullOrWhiteSpace(videoPlayer.url)
                        || videoPlayer.clip != null;
                }

                return false;
            }
        }

        public event EventHandler OnPrepared;
        public event EventHandler OnStarted;
        public event EventHandler OnLoopPointReached;
        public event EventHandler<string> OnErrorReceived;

        protected void Awake()
        {
            videoPlayer = this.RequireComponentInstance<VideoPlayer>();
            rawImage = this.RequireComponentInstance<RawImage>();

            if (videoPlayer == null)
            {
                enabled = false;
                return;
            }
            else
            {
                videoPlayer.enabled = true;
            }

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

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;

            rawImage.texture = renderTexture;

            videoPlayer.prepareCompleted += VideoPlayerPrepareCompleted;
            videoPlayer.started += VideoPlayerStarted;
            videoPlayer.loopPointReached += VideoPlayerLoopPointReached;
            videoPlayer.errorReceived += VideoPlayerErrorReceived;

            notifications = new List<INotification>();
        }

        private void Update()
        {
            UpdateNotifications();
        }

        private void OnEnable()
        {
            rawImage.enabled = true;
            Play();
        }

        private void OnDisable()
        {
            rawImage.enabled = false;
            Pause();
        }

        private void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPrepareCompleted;
            videoPlayer.started -= VideoPlayerStarted;
            videoPlayer.loopPointReached -= VideoPlayerLoopPointReached;
            videoPlayer.errorReceived -= VideoPlayerErrorReceived;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            var rawImageComponent = GetComponent<RawImage>();
            if (rawImageComponent != null)
            {
                rawImageComponent.texture = editorPreview;
            }
#endif
        }

        private void VideoPlayerPrepareCompleted(VideoPlayer source)
        {
            IList<UniTaskCompletionSource<Video>> taskCompletionSources;
            lock (onPrepared)
            {
                taskCompletionSources = onPrepared.Copy();
                onPrepared.Clear();
            }

            taskCompletionSources.ForEach((taskCompletionSource, index) =>
            {
                taskCompletionSource.TrySetResult(this);
            });
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

        private void VideoPlayerErrorReceived(VideoPlayer source, string error)
        {
            Debug.LogError(error, this);
            OnErrorReceived?.Invoke(this, error);
        }

        public void Prepare()
        {
            if (IsAssignedContent)
            {
                videoPlayer.Prepare();
            }
        }

        [Button("Play")]
        public void Play()
        {
            if (IsAssignedContent)
            {
                videoPlayer.Play();
            }
        }

        [Button("Pause")]
        public void Pause()
        {
            videoPlayer.Pause();
        }

        [Button("Stop")]
        public void Stop()
        {
            videoPlayer.Stop();
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
                videoPlayer.source = VideoSource.VideoClip;
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
                videoPlayer.source = VideoSource.Url;
                return PrepareAfterSet();
            }
            else
            {
                Stop();
                return UniTask.CompletedTask;
            }
        }

        public void AddNotificationAtPercent(Action<IVideo> perform, float atPercent)
        {
            notifications.Add(new AtPercentNotification()
            {
                perform = perform,
                atPercent = atPercent
            });
        }

        public void AddNotificationAtTimeFromEnd(Action<IVideo> perform, float atTimeFromEnd)
        {
            Debug.Log($"{gameObject.name} Notification added at time from end {atTimeFromEnd}");
            notifications.Add(new AtTimeFromEndNotification()
            {
                perform = perform,
                atTimeFromEnd = atTimeFromEnd
            });
        }

        private void PerformAndRemove(INotification perform)
        {
            perform.perform.Invoke(this);
            var removeResult = notifications.Remove(perform);
            if (!removeResult)
            {
                Debug.LogError("Unable to remove performed notification, will likely trigger again unexpectedly", this);
            }
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

        [Button]
        private void SaveCurrentFrameToDisk()
        {
            if (rawImage == null || rawImage.texture == null || videoPlayer == null)
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
                $"{Application.temporaryCachePath}/Video_{gameObject.name}-{gameObject.GetInstanceID()}_frame-{videoPlayer.frame}.png";
            rawImageTexture.WritePNGToDisk(savePath);
            Debug.Log($"Current Frame saved to '{savePath}'", this);
        }

        public void SetRenderTextureSize(Vector2 size)
        {
            var resized = new RenderTexture((int)size.x, (int)size.y, renderTexture.depth, renderTexture.format);
            resized.name = this.name;
            resized.Create();

            videoPlayer.targetTexture = resized;
            rawImage.texture = resized;
            renderTexture = resized;
        }

        private interface INotification
        {
            Action<Video> perform { get; }
        }

        [Serializable]
        private struct AtPercentNotification : INotification
        {
            public float atPercent;
            public Action<Video> perform { get; set; }

            public override string ToString()
            {
                // ReSharper disable once HeapView.BoxingAllocation
                return $"At percent: {atPercent}%";
            }
        }

        [Serializable]
        private struct AtTimeFromEndNotification : INotification
        {
            public float atTimeFromEnd;
            public Action<Video> perform { get; set; }

            public override string ToString()
            {
                // ReSharper disable once HeapView.BoxingAllocation
                return $"At time from end: {atTimeFromEnd} seconds";
            }
        }
    }
}