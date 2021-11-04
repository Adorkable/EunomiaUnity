using System;
using Eunomia;
using UnityEngine;

namespace EunomiaUnity.UI
{
    public class OverlappingVideos : MonoBehaviour
    {
        [SerializeField]
        private Video first;
        [SerializeField]
        private Video second;

        private Video[] videos;
        private int currentIndex = 0;
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
        private Video Next
        {
            get
            {
                return videos[(currentIndex + 1).Wrap(videos.Length)];
            }
        }
        [SerializeField]
        private float overlapDuration = 0.25f;
        public float OverlapDuration
        {
            get
            {
                return overlapDuration;
            }
            set
            {
                overlapDuration = value;
            }
        }

        public event EventHandler OnCurrentReachedLoopPoint;

        protected void Awake()
        {
            if (first == null || second == null)
            {
                Debug.LogError("Expect two Video references to function correctly", this);
                enabled = false;
                return;
            }

            videos = new Video[] { first, second };
            videos.ForEach((video, index) =>
            {
                video.OnLoopPointReached += VideoLoopPointReached;
            });
        }

        void OnDestroy()
        {
            videos.ForEach((video, index) =>
            {
                video.OnLoopPointReached -= VideoLoopPointReached;
            });
        }

        private void VideoLoopPointReached(object video, EventArgs args)
        {
            if (overlapDuration > 0)
            {
                Current.Stop();

                if (String.IsNullOrWhiteSpace(Next.Url))
                {
                    Next.Stop();
                    Next.Url = Current.Url;
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

            OnCurrentReachedLoopPoint?.Invoke(this, args);
        }

        // TODO: work out what happens if next finishes before current
        private void OverlapNext()
        {
            if (String.IsNullOrWhiteSpace(Next.Url))
            {
                Next.Url = Current.Url;
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
        }

        public void SetNextUrl(string url)
        {
            if (String.IsNullOrWhiteSpace(Current.Url))
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
                if (String.IsNullOrWhiteSpace(Current.Url) == false)
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
