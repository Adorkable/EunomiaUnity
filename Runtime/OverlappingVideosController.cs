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

        [SerializeField, Label("Third"), ShowIf("VideoTypeIsWorld")]
        private Video thirdWorld;

        [SerializeField, Label("First"), ShowIf("VideoTypeIsUI")]
        private UI.Video firstUI;

        [SerializeField, Label("Second"), ShowIf("VideoTypeIsUI")]
        private UI.Video secondUI;

        [SerializeField, Label("Third"), ShowIf("VideoTypeIsUI")]
        private UI.Video thirdUI;

        [SerializeField]
        private float overlapDuration = 0.5f;

        [SerializeField]
        private bool crossfadeOverlap = true;

        private volatile int currentIndex;

        private IVideo[] videos;

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
                lock (this)
                {
                    if (videos == null || videos.Length <= currentIndex)
                    {
                        return null;
                    }

                    return videos[currentIndex];
                }
            }
        }

        [ShowNativeProperty]
        private IVideo Next
        {
            get
            {
                lock (this)
                {
                    return videos[(currentIndex + 1).Wrap(videos.Length)];
                }
            }
        }

        [ShowNativeProperty]
        private IVideo Last
        {
            get
            {
                lock (this)
                {
                    return videos[(currentIndex - 1).Wrap(videos.Length)];
                }
            }
        }

        private bool nextSet = false;
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

                    videos = new[] { firstWorld, secondWorld, thirdWorld };
                    break;

                case VideoType.UI:
                    if (firstUI == null || secondUI == null)
                    {
                        Debug.LogError("Expect two Video references to function correctly", this);
                        enabled = false;
                        return;
                    }

                    videos = new[] { firstUI, secondUI, thirdUI };
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
            lock (this)
            {
                currentIndex = (currentIndex + 1).Wrap(videos.Length);
            }
        }

        private async UniTask LoopWithOverlap()
        {
            Last.Stop();

            await EnsureNextPrepared();
        }

        private UniTask ImmediatelyGoToNext()
        {
            return OverlapNext();
        }

        private void VideoLoopPointReached(object video, EventArgs args)
        {
            if (overlapDuration > 0)
            {
                _ = LoopWithOverlap();
            }
            else
            {
                Current.Stop();
                _ = ImmediatelyGoToNext();
            }

            OnReachedLoopPoint?.Invoke(this, args);
        }

        private UniTask ForceNextPrepared()
        {
            UniTask result;
            Next.Stop();
            Next.IsLooping = false;

            switch (Current.ContentSource)
            {
                case VideoSource.VideoClip:
                    result = Next.SetVideoClip(Current.VideoClip);
                    nextSet = true;
                    break;

                case VideoSource.Url:
                    result = Next.SetUrl(Current.Url);
                    nextSet = true;
                    break;

                default:
                    result = UniTask.CompletedTask;
                    break;
            }
            return result;
        }

        private UniTask EnsureNextPrepared()
        {
            UniTask result;
            if (!Next.IsAssignedContent || nextSet == false)
            {
                result = ForceNextPrepared();
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
            else
            {
                Debug.LogError("Received Overlap Time notification for Next", this);
            }

            OnReachedOverlapPoint?.Invoke(this, EventArgs.Empty);
        }

        // TODO: work out what happens if next finishes before current
        private async UniTask OverlapNext()
        {
            var prepareTask = EnsureNextPrepared();

            MakeNextBecomeCurrent();
            nextSet = false;

            if (Current.IsAssignedContent)
            {
                // TODO: double check not already registered
                currentNotification = Current.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);

                await prepareTask;
                MainThreadDispatcher.Invoke(() => Current.Play());
            }
            else
            {
                Debug.LogError("Somehow Next was unable to ensure content", this);
            }
        }

        public void SetNextVideoClip(VideoClip videoClip)
        {
            if (!Current.IsAssignedContent)
            {
                Current.SetVideoClip(videoClip);
            }
            else
            {
                Next.SetVideoClip(videoClip);
                nextSet = true;
            }
        }

        public void SetNextUrl(string url)
        {
            if (!Current.IsAssignedContent)
            {
                Current.SetUrl(url);
            }
            else
            {
                Next.SetUrl(url);
                nextSet = true;
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
            Current.Stop();
            if (currentNotification != null)
            {
                Current.CancelNotification(currentNotification);
                currentNotification = null;
            }
            Next.Stop();
            Last.Stop();
        }

        private void VideoErrorReceived(object video, string error)
        {
            OnErrorReceived?.Invoke(video, error);
        }
    }
}