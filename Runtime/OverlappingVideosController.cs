using System;
using Eunomia;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Video;

// TODO: rename to Overlapping Videos Controller or something of the like to clarify its role
// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public interface IVideo
    {
        bool IsPlaying { get; }
        bool PlayOnAwake { get; set; }
        bool IsLooping { get; set; }

        bool IsAssignedContent { get; }
        VideoClip VideoClip { get; set; }
        string Url { get; set; }

        double Duration { get; }
        double Time { get; set; }

        Vector2Int Size { get; }

        event EventHandler OnLoopPointReached;

        void Play();
        void Prepare();
        void Pause();
        void Stop();

        void AddNotificationAtPercent(Action<IVideo> perform, float atPercent);
        void AddNotificationAtTimeFromEnd(Action<IVideo> perform, float atTimeFromEnd);
    }

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
            });
        }

        private void OnDestroy()
        {
            videos.ForEach((video, index) => { video.OnLoopPointReached -= VideoLoopPointReached; });
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            videos = new[] { firstWorld, secondWorld };
#endif
        }

        public event EventHandler OnReachedLoopPoint;
        public event EventHandler OnReachedOverlapPoint;

        private void VideoLoopPointReached(object video, EventArgs args)
        {
            if (overlapDuration > 0)
            {
                Current.Stop();

                if (!Next.IsAssignedContent)
                {
                    Next.Stop();
                    if (Current.VideoClip != null)
                    {
                        Next.VideoClip = Current.VideoClip;
                    }
                    else
                    {
                        Next.Url = Current.Url;
                    }
                }

                currentIndex = (currentIndex + 1).Wrap(videos.Length);
                if (Current.IsPlaying == false)
                {
                    Current.Play();
                    Current.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);
                }
            }
            else
            {
                if (Current.IsLooping == false)
                {
                    Current.IsLooping = true;
                    Current.Play();
                }
            }

            OnReachedLoopPoint?.Invoke(this, args);
        }

        // TODO: work out what happens if next finishes before current
        private void OverlapNext()
        {
            if (!Next.IsAssignedContent)
            {
                if (Current.VideoClip != null)
                {
                    Next.VideoClip = Current.VideoClip;
                }
                else
                {
                    Next.Url = Current.Url;
                }
            }

            Next.Play();
            if (overlapDuration > 0)
            {
                Next.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);
            }
        }

        private void HandleAtOverlapTime(IVideo source)
        {
            if (source != Next)
            {
                OverlapNext();
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
                    Current.Play();
                    if (overlapDuration > 0)
                    {
                        Current.AddNotificationAtTimeFromEnd(HandleAtOverlapTime, overlapDuration);
                    }
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
        }
    }
}