using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eunomia;
using EunomiaUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace EunomiaUnity.UI
{
    [RequireComponent(typeof(VideoPlayer)), RequireComponent(typeof(RawImage))]
    public class Video : MonoBehaviour
    {
        private VideoPlayer videoPlayer;
        private RawImage rawImage;
        [SerializeField]
        private RenderTexture renderTextureTemplate;
        private RenderTexture renderTexture;

        [SerializeField]
        private Texture2D editorPreview;

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
            set
            {
                videoPlayer.isLooping = value;
            }
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
            set
            {
                _ = SetUrl(value);
            }
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

                return new Vector2Int(videoPlayer.texture.width, videoPlayer.texture.height);
            }
        }

        private List<UniTaskCompletionSource<Video>> onPrepared = new List<UniTaskCompletionSource<Video>>();

        public event EventHandler OnPrepared;
        public event EventHandler OnStarted;
        public event EventHandler OnLoopPointReached;

        private interface INotification
        {
            Action<Video> perform { get; }
        }
        [Serializable]
        private struct AtPercentNotification : INotification
        {
            public Action<Video> perform { get; set; }
            public float atPercent;

            public override string ToString()
            {
                return $"At percent: {atPercent}%";
            }
        }
        [Serializable]
        private struct AtTimeFromEndNotification : INotification
        {
            public Action<Video> perform { get; set; }
            public float atTimeFromEnd;

            public override string ToString()
            {
                return $"At time from end: {atTimeFromEnd} seconds";
            }
        }
        [Foldout("Debug"), AllowNesting, NaughtyAttributes.ReadOnly, SerializeField]
        private List<INotification> notifications;

        protected void Awake()
        {
            videoPlayer = this.RequireComponentInstance<VideoPlayer>();
            rawImage = this.RequireComponentInstance<RawImage>();

            if (videoPlayer == null)
            {
                enabled = false;
                return;
            }
            if (rawImage == null)
            {
                enabled = false;
                return;
            }
            if (renderTextureTemplate == null)
            {
                this.LogMissingRequiredReference(typeof(VideoPlayer));
                enabled = false;
                return;
            }

            var rawImageTransform = rawImage.GetComponent<RectTransform>();
            renderTexture = new RenderTexture(renderTextureTemplate);
            renderTexture.name = this.name;
            renderTexture.Create();

            videoPlayer.renderMode = VideoRenderMode.RenderTexture;
            videoPlayer.targetTexture = renderTexture;

            rawImage.texture = renderTexture;

            videoPlayer.prepareCompleted += VideoPlayerPrepareCompleted;
            videoPlayer.started += VideoPlayerStarted;
            videoPlayer.loopPointReached += VideoPlayerLoopPointReached;
            videoPlayer.errorReceived += VideoPlayerErrorRecieved;

            notifications = new List<INotification>();
        }

        void OnDestroy()
        {
            videoPlayer.prepareCompleted -= VideoPlayerPrepareCompleted;
            videoPlayer.started -= VideoPlayerStarted;
            videoPlayer.loopPointReached -= VideoPlayerLoopPointReached;
            videoPlayer.errorReceived -= VideoPlayerErrorRecieved;
        }

        void OnEnable()
        {
            rawImage.enabled = true;
            Play();
        }

        void OnDisable()
        {
            rawImage.enabled = false;
            Pause();
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
            OnPrepared?.Invoke(this, new EventArgs());
        }

        private void VideoPlayerStarted(VideoPlayer source)
        {
            OnStarted?.Invoke(this, new EventArgs());
        }

        private void VideoPlayerLoopPointReached(VideoPlayer source)
        {
            OnLoopPointReached?.Invoke(this, new EventArgs());
        }

        private void VideoPlayerErrorRecieved(VideoPlayer source, string error)
        {
            Debug.LogError(error, this);
            VideoPlayerLoopPointReached(source);
        }

        public void Prepare()
        {
            if (String.IsNullOrWhiteSpace(videoPlayer.url) == false)
            {
                videoPlayer.Prepare();
            }
        }

        public void Play()
        {
            if (String.IsNullOrWhiteSpace(videoPlayer.url) == false)
            {
                videoPlayer.Play();
            }
        }

        public void Pause()
        {
            videoPlayer.Pause();
        }

        public void Stop()
        {
            videoPlayer.Stop();
        }

        public UniTask SetUrl(string url)
        {
            videoPlayer.url = url;
            if (String.IsNullOrWhiteSpace(videoPlayer.url) == false)
            {
                videoPlayer.Prepare();

                var prepareTask = new UniTaskCompletionSource<Video>();
                lock (onPrepared)
                {
                    onPrepared.Add(prepareTask);
                }
                return prepareTask.Task;
            }
            else
            {
                videoPlayer.Stop();
                return UniTask.CompletedTask;
            }
        }

        public void AddNotificationAtPercent(Action<Video> perform, float atPercent)
        {
            notifications.Add(new AtPercentNotification()
            {
                perform = perform,
                atPercent = atPercent
            });
        }

        public void AddNotificationAtTimeFromEnd(Action<Video> perform, float atTimeFromEnd)
        {
            notifications.Add(new AtTimeFromEndNotification()
            {
                perform = perform,
                atTimeFromEnd = atTimeFromEnd
            });
        }

        void PerformAndRemove(INotification perform)
        {
            perform.perform.Invoke(this);
            notifications.Remove(perform);
        }

        void UpdateNotifications()
        {
            if (videoPlayer.length == 0)
            {
                return;
            }

            int index = 0;
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

        void Update()
        {
            UpdateNotifications();
        }

        [Button]
        void SaveCurrentFrameToDisk()
        {
            if (rawImage == null || rawImage.texture == null || videoPlayer == null)
            {
                Debug.LogWarning("Raw Image, Raw Image's texture or Video Player not found, unable to Save Current Frame To Disk", this);
                return;
            }
            var rawImageTexture = (RenderTexture)rawImage.texture;
            if (rawImageTexture == null)
            {
                Debug.LogWarning($"Unexpected texture type '{rawImage.texture.GetType()}'", this);
                return;
            }

            var savePath = $"{Application.temporaryCachePath}/Video_{gameObject.name}-{gameObject.GetInstanceID()}_frame-{videoPlayer.frame}.png";
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

        void OnValidate()
        {
#if UNITY_EDITOR
            var rawImage = GetComponent<RawImage>();
            if (rawImage != null)
            {
                rawImage.texture = editorPreview;
            }
#endif
        }
    }
}