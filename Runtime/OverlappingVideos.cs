using System;
using Eunomia;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Video;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    [AddComponentMenu("Video/Overlapping Videos")]
    // TODO: test with Current and Next swapping between using Url and VideoClip
    public class OverlappingVideos : MonoBehaviour
    {
        [SerializeField] private Video first;

        [SerializeField] private Video second;

        [SerializeField] private float overlapDuration = 0.5f;

        [SerializeField] private bool crossfadeOverlap = true;

        private int currentIndex;

        private Video[] videos;

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
        private Video Current
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
        private Video Next
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
            if (first == null || second == null)
            {
                Debug.LogError("Expect two Video references to function correctly", this);
                enabled = false;
                return;
            }

            videos = new[] {first, second};
            videos.ForEach((video, index) =>
            {
                if (crossfadeOverlap)
                {
                    video.fadeInDuration = overlapDuration;
                    video.fadeOutDuration = overlapDuration;
                }
                else
                {
                    video.fadeInDuration = 0;
                    video.fadeOutDuration = 0;
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
            videos = new[] {first, second};
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

        private void HandleAtOverlapTime(Video source)
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