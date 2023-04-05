using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Eunomia;
using EunomiaUnity;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Video;

[RequireComponent(typeof(VideoPlayer))]
public abstract class VideoBase : MonoBehaviour, IVideo
{
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
    public bool IsPrepared
    {
        get
        {
            if (videoPlayer != null)
            {
                return videoPlayer.isPrepared;
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
    public long Frame
    {
        get
        {
            if (videoPlayer != null)
            {
                return videoPlayer.frame;
            }

            return 0;
        }
        set
        {
            if (videoPlayer != null)
            {
                videoPlayer.frame = value;
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

    public VideoRenderMode RenderMode
    {
        get
        {
            if (videoPlayer == null)
            {
                // TODO: throw
                return VideoRenderMode.RenderTexture;
            }

            return videoPlayer.renderMode;
        }
        set
        {
            if (videoPlayer != null)
            {
                videoPlayer.renderMode = value;
            }
        }
    }

    public RenderTexture TargetTexture
    {
        get
        {
            if (videoPlayer == null)
            {
                return null;
            }

            return videoPlayer.targetTexture;
        }
        set
        {
            if (videoPlayer != null)
            {
                videoPlayer.targetTexture = value;
            }
        }
    }

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
            bool result;
            if (videoPlayer != null)
            {
                switch (ContentSource)
                {
                    case VideoSource.VideoClip:
                        result = VideoClip != null;
                        break;

                    case VideoSource.Url:
                        result = String.IsNullOrWhiteSpace(Url) == false;
                        break;

                    default:
                        result = false;
                        break;
                }
            }
            else
            {
                result = false;
            }

            return result;
        }
    }

    public VideoSource ContentSource
    {
        get
        {
            if (videoPlayer != null)
            {
                return videoPlayer.source;
            }
            // TODO: throw
            return VideoSource.VideoClip;
        }
        set
        {
            if (videoPlayer != null)
            {
                videoPlayer.source = value;
            }

        }
    }

    public event EventHandler OnPrepared;
    public event EventHandler OnStarted;
    public event EventHandler OnLoopPointReached;
    public event EventHandler<string> OnErrorReceived;

    private readonly List<UniTaskCompletionSource<IVideo>> onPrepared = new List<UniTaskCompletionSource<IVideo>>();

    // ReSharper disable once Unity.RedundantSerializeFieldAttribute - not obvious on first glance
    [SerializeField, Foldout("Debug"), AllowNesting, ReadOnly]
    private List<IVideoNotification> notifications;

    protected virtual void Awake()
    {
        videoPlayer = this.RequireComponentInstance<VideoPlayer>();

        if (videoPlayer == null)
        {
            enabled = false;
            return;
        }
        else
        {
            videoPlayer.enabled = true;
        }

        videoPlayer.prepareCompleted += VideoPlayerPrepareCompleted;
        videoPlayer.started += VideoPlayerStarted;
        videoPlayer.loopPointReached += VideoPlayerLoopPointReached;
        videoPlayer.errorReceived += VideoPlayerErrorReceived;

        notifications = new List<IVideoNotification>();
    }

    protected virtual void Update()
    {
        UpdateNotifications();
    }

    protected virtual void OnEnable()
    {
        Play();
    }

    protected virtual void OnDisable()
    {
        Pause();
    }

    protected virtual void OnDestroy()
    {
        videoPlayer.prepareCompleted -= VideoPlayerPrepareCompleted;
        videoPlayer.started -= VideoPlayerStarted;
        videoPlayer.loopPointReached -= VideoPlayerLoopPointReached;
        videoPlayer.errorReceived -= VideoPlayerErrorReceived;
    }

    private void VideoPlayerPrepareCompleted(VideoPlayer source)
    {
        IList<UniTaskCompletionSource<IVideo>> taskCompletionSources;
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

    System.Threading.Thread StartThread;
    void Start()
    {
        StartThread = System.Threading.Thread.CurrentThread;
    }

    [Button("Play")]
    public virtual void Play()
    {
        if (IsAssignedContent)
        {
            videoPlayer.Play();
        }
    }

    [Button("Pause")]
    public virtual void Pause()
    {
        videoPlayer.Pause();
    }

    [Button("Stop")]
    public virtual void Stop()
    {
        videoPlayer.Stop();
    }

    private UniTask PrepareAfterSet()
    {
        videoPlayer.Prepare();

        var prepareTask = new UniTaskCompletionSource<IVideo>();
        lock (onPrepared)
        {
            onPrepared.Add(prepareTask);
        }

        return prepareTask.Task;
    }

    public UniTask SetVideoClip(VideoClip videoClip)
    {
        if (videoClip != null)
        {
            videoPlayer.source = VideoSource.VideoClip;
            videoPlayer.clip = videoClip;
            return PrepareAfterSet();
        }
        else
        {
            videoPlayer.clip = null;
            Stop();
            return UniTask.CompletedTask;
        }
    }

    public UniTask SetUrl(string url)
    {
        if (String.IsNullOrWhiteSpace(url) == false)
        {
            videoPlayer.source = VideoSource.Url;
            videoPlayer.url = url;
            return PrepareAfterSet();
        }
        else
        {
            videoPlayer.url = "";
            Stop();
            return UniTask.CompletedTask;
        }
    }

    public void SetPlaybackSpeed(float playbackSpeed)
    {
        videoPlayer.playbackSpeed = playbackSpeed;
    }

    public IVideoNotification AddNotificationAtPercent(Action<IVideo> perform, float atPercent)
    {
        var notification = new AtPercentNotification()
        {
            perform = perform,
            atPercent = atPercent
        };
        notifications.Add(notification);
        return notification;
    }

    public IVideoNotification AddNotificationAtTimeFromEnd(Action<IVideo> perform, float atTimeFromEnd)
    {
        var notification = new AtTimeFromEndNotification()
        {
            perform = perform,
            atTimeFromEnd = atTimeFromEnd
        };
        notifications.Add(notification);
        return notification;
    }

    private void PerformAndRemove(IVideoNotification perform)
    {
        perform.perform.Invoke(this);
        var removeResult = notifications.Remove(perform);
        if (!removeResult)
        {
            Debug.LogError("Unable to remove performed notification, will likely trigger again unexpectedly", this);
        }
    }

    public void CancelNotification(IVideoNotification notification)
    {
        var removeResult = notifications.Remove(notification);
        if (!removeResult)
        {
            // TODO: throw
            Debug.LogError("Unable to cancel notification", this);
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

    [Serializable]
    private struct AtPercentNotification : IVideoNotification
    {
        public float atPercent;
        public Action<IVideo> perform { get; set; }

        public override string ToString()
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return $"At percent: {atPercent}%";
        }
    }

    [Serializable]
    private struct AtTimeFromEndNotification : IVideoNotification
    {
        public float atTimeFromEnd;
        public Action<IVideo> perform { get; set; }

        public override string ToString()
        {
            // ReSharper disable once HeapView.BoxingAllocation
            return $"At time from end: {atTimeFromEnd} seconds";
        }
    }

    protected RenderTexture CurrentRenderTexture()
    {
        if (videoPlayer == null)
        {
            // TODO: throw
            return null;
        }

        return videoPlayer.CurrentRenderTexture();
    }

    protected virtual void OnValidate()
    {
#if UNITY_EDITOR
        videoPlayer = this.RequireComponentInstance<VideoPlayer>();
#endif
    }
}