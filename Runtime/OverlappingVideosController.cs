using System;
using Cysharp.Threading.Tasks;
using Eunomia;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Video;

// TODO: rename to Overlapping Videos Controller or something of the like to clarify its role
// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [AddComponentMenu("Video/Overlapping Videos Controller")]
    // TODO: test with Current and Next swapping between using Url and VideoClip
    public class OverlappingVideosController : MonoBehaviour
    {
        [Serializable]
        enum VideoType
        {
            World,
            UI
        }
        [SerializeField]
        private VideoType videoType;

        public bool VideoTypeIsWorld => videoType == VideoType.World;
        public bool VideoTypeIsUI => videoType == VideoType.UI;

        [SerializeField, Label("First"), ShowIf("VideoTypeIsWorld")]
        private Video firstWorld;

        [SerializeField, Label("Second"), ShowIf("VideoTypeIsWorld")]
        private Video secondWorld;

        [SerializeField, Label("First"), ShowIf("VideoTypeIsUI")]
        private UI.Video firstUI;

        [SerializeField, Label("Second"), ShowIf("VideoTypeIsUI")]
        private UI.Video secondUI;

        [SerializeField]
        private float overlapDuration = 0.5f;

        [SerializeField]
        private bool crossfadeOverlap = true;

        private int currentIndex;

        private IVideo[] videos;

        // [SerializeField]
        // private bool allowLoopUntilNextPrepared = false;

        public float OverlapDuration
        {
            get => overlapDuration;
            set => overlapDuration = value;
        }

        [ShowNativeProperty]
        public bool IsPlaying
        {
            get
            {
                if (Current != null)
                {
                    return Current.IsPlaying;
                }

                return false;
            }
        }

        [ShowNativeProperty]
        private IVideo Current
        {
            get
            {
                if (videos == null || videos.Length <= currentIndex)
                {
                    return null;
                }

                return videos[currentIndex];
            }
        }

        [ShowNativeProperty]
        public VideoClip CurrentVideoClip
        {
            get
            {
                if (Current != null)
                {
                    return Current.VideoClip;
                }

                return null;
            }
        }

        [ShowNativeProperty]
        public string CurrentUrl
        {
            get
            {
                if (Current != null)
                {
                    return Current.Url;
                }

                return null;
            }
        }

        [ShowNativeProperty]
        public double CurrentDuration
        {
            get
            {
                if (Current != null)
                {
                    return Current.Duration;
                }

                return 0;
            }
        }

        [ShowNativeProperty]
        public double CurrentTime
        {
            get
            {
                if (Current != null)
                {
                    return Current.Time;
                }

                return 0;
            }
        }

        [ShowNativeProperty]
        public Vector2Int CurrentSize
        {
            get
            {
                if (Next != null)
                {
                    return Next.Size;
                }

                return new Vector2Int(0, 0);
            }
        }

        [ShowNativeProperty]
        private IVideo Next
        {
            get { return videos[(currentIndex + 1).Wrap(videos.Length)]; }
        }

        [ShowNativeProperty]
        public VideoClip NextVideoClip
        {
            get
            {
                if (Next != null)
                {
                    return Next.VideoClip;
                }

                return null;
            }
        }

        [ShowNativeProperty]
        public string NextUrl
        {
            get
            {
                if (Next != null)
                {
                    return Next.Url;
                }

                return null;
            }
        }

        [ShowNativeProperty]
        public double NextDuration
        {
            get
            {
                if (Next != null)
                {
                    return Next.Duration;
                }

                return 0;
            }
        }

        [ShowNativeProperty]
        public double NextTime
        {
            get
            {
                if (Next != null)
                {
                    return Next.Time;
                }

                return 0;
            }
        }

        [ShowNativeProperty]
        public Vector2Int NextSize
        {
            get
            {
                if (Next != null)
                {
                    return Next.Size;
                }

                return new Vector2Int(0, 0);
            }
        }

        private IVideoNotification currentNotification;

        public event EventHandler OnReachedLoopPoint;
        public event EventHandler OnReachedOverlapPoint;
        public event EventHandler<string> OnErrorReceived; // TODO: way to tell receiver if it was Current or Next

        // TODO: rethink this ownership
        private MainThreadDispatcher MainThreadDispatcher
        {
            get
            {
                if (mainThreadDispatcher == null)
                {
                    mainThreadDispatcher = GetComponent<MainThreadDispatcher>();
                    if (mainThreadDispatcher == null)
                    {
                        mainThreadDispatcher = gameObject.AddComponent<MainThreadDispatcher>();
                    }
                }
                return mainThreadDispatcher;
            }
        }
        private MainThreadDispatcher mainThreadDispatcher;

        protected void Awake()
        {
            switch (videoType)
            {
                case VideoType.World:
                    if (firstWorld == null || secondWorld == null)
                    {
                        Debug.LogError("Expect two Video references to function correctly", this);
                        enabled = false;
                        return;
                    }

                    videos = new[] { firstWorld, secondWorld };
                    break;

                case VideoType.UI:
                    if (firstUI == null || secondUI == null)
                    {
                        Debug.LogError("Expect two Video references to function correctly", this);
                        enabled = false;
                        return;
                    }

                    videos = new[] { firstUI, secondUI };
                    break;
            }

            videos.ForEach((video, index) =>
            {
                if (video is Video worldVideo)
                {
                    if (crossfadeOverlap)
                    {
                        worldVideo.fadeInDuration = overlapDuration;
                        worldVideo.fadeOutDuration = overlapDuration;
                    }
                    else
                    {
                        worldVideo.fadeInDuration = 0;
                        worldVideo.fadeOutDuration = 0;
                    }
                }

                video.PlayOnAwake = false;
                video.OnLoopPointReached += VideoLoopPointReached;
                video.OnErrorReceived += VideoErrorReceived;
            });
        }

        private void OnDestroy()
        {
            // TODO: cancel notification
            videos.ForEach((video, index) => { video.OnLoopPointReached -= VideoLoopPointReached; });
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            videos = new[] { firstWorld, secondWorld };
#endif
        }

        private void MakeNextBecomeCurrent()
        {
            currentIndex = (currentIndex + 1).Wrap(videos.Length);
        }

        private async UniTask EnsureOverlap()
        {
            Current.Stop();

            var prepareTask = EnsureNextPrepared();

            MakeNextBecomeCurrent();

            if (Current.IsPlaying == false)
            {
                if (overlapDuration > 0)
                {
                    if (currentNotification == null)
                    {
                        currentNotification = Current.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);
                    }
                }
                await prepareTask;
                MainThreadDispatcher.Invoke(() => Current.Play());
            }

            await EnsureNextPrepared();
        }

        private void LoopCurrent()
        {
            if (Current.IsLooping == false)
            {
                Current.IsLooping = true;
                MainThreadDispatcher.Invoke(() => Current.Play());
            }
        }

        private void VideoLoopPointReached(object video, EventArgs args)
        {
            if (overlapDuration > 0)
            {
                _ = EnsureOverlap();
            }
            else
            {
                LoopCurrent();
            }

            OnReachedLoopPoint?.Invoke(this, args);
        }

        // TODO: work out what happens if next finishes before current
        private async UniTask OverlapNext()
        {
            var prepareTask = EnsureNextPrepared();

            if (Next.IsAssignedContent)
            {
                if (overlapDuration > 0)
                {
                    // TODO: double check not already registered
                    currentNotification = Next.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);
                }

                await prepareTask;
                MainThreadDispatcher.Invoke(() => Next.Play());
            }
            else
            {
                Debug.LogError("Somehow Next was unable to ensure content", this);
            }
        }

        private UniTask EnsureNextPrepared()
        {
            UniTask result;
            if (!Next.IsAssignedContent)
            {
                Next.Stop();
                switch (Current.ContentSource)
                {
                    case VideoSource.VideoClip:
                        result = Next.SetVideoClip(Current.VideoClip);
                        break;

                    case VideoSource.Url:
                        result = Next.SetUrl(Current.Url);
                        break;

                    default:
                        result = UniTask.CompletedTask;
                        break;
                }
            }
            else
            {
                result = UniTask.CompletedTask;
            }
            return result;
        }

        private void HandleAtOverlapTime(IVideo source)
        {
            if (source != Next)
            {
                currentNotification = null;
                _ = OverlapNext();
            }

            OnReachedOverlapPoint?.Invoke(this, EventArgs.Empty);
        }

        public void SetNextVideoClip(VideoClip videoClip)
        {
            if (!Current.IsAssignedContent)
            {
                Current.VideoClip = videoClip;
                Current.Prepare();
            }
            else
            {
                Next.VideoClip = videoClip;
            }
        }

        public void SetNextUrl(string url)
        {
            if (!Current.IsAssignedContent)
            {
                Current.Url = url;
                Current.Prepare();
            }
            else
            {
                Next.Url = url;
            }
        }

        public void Play()
        {
            if (Current != null)
            {
                if (Current.IsAssignedContent)
                {
                    if (overlapDuration > 0)
                    {
                        // TODO: double check we haven't already registered
                        currentNotification = Current.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);
                    }
                    MainThreadDispatcher.Invoke(() => Current.Play());
                }
            }
            else
            {
                throw new InvalidOperationException("Has not initialized yet");
            }
        }

        public void Pause()
        {
            Current.Pause();
        }

        public void Stop()
        {
            // TODO: Stop should clear all notifications since we're adding every Play
            Current.Stop();
        }

        private void VideoErrorReceived(object video, string error)
        {
            OnErrorReceived?.Invoke(video, error);
        }
    }
}