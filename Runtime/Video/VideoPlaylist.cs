using Eunomia;
using EunomiaUnity;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace EunomiaUnity
{
    [RequireComponent(typeof(Video))]
    public class VideoPlaylist : MonoBehaviour
    {
        Video video;

        [Serializable]
        struct ClipWithPlaySettings
        {
            public VideoClip clip;
            public VideoClip[] clipSet; // TODO: hacky, come up with better solution
            public float playbackSpeed;

            public PublisherUnityEvent OnPlay;
        }

        [SerializeField]
        private ClipWithPlaySettings[] clips;
        private int currentClipIndex = 0;

        enum PlayOrder
        {
            Forward,
            Reverse,
            Random,
            RandomWithoutReplace
        }
        [SerializeField]
        private PlayOrder playOrder = PlayOrder.Random;
        private List<int> unreplacedIndices;

        [SerializeField]
        private float fadeOutAtTimeFromEnd = 2;
        [SerializeField]
        private PublisherUnityEvent FadeOut;

        [SerializeField]
        private float fadeInAtTimeAfterBeginning = 2;
        [SerializeField]
        private PublisherUnityEvent FadeIn;

        enum Loop
        {
            None,
            All,
            Single
        }
        [SerializeField]
        private Loop loop;

        void Awake()
        {
            video = GetComponent<Video>();
            video.OnLoopPointReached += HandleLoopPointReached;
        }

        void Start()
        {
            PlayCurrentClip();
        }

        void HandleLoopPointReached(object sender, EventArgs e)
        {
            GoToNextAndPlay();
        }

        [Button]
        void GoToNextAndPlay()
        {
            if (loop != Loop.Single)
            {
                switch (playOrder)
                {
                    case PlayOrder.Forward:
                        currentClipIndex += 1;
                        if (currentClipIndex >= clips.Length)
                        {
                            currentClipIndex = 0;
                            if (loop == Loop.None)
                            {
                                video.Stop();
                                return;
                            }
                        }
                        break;
                    case PlayOrder.Reverse:
                        currentClipIndex -= 1;
                        if (currentClipIndex < 0)
                        {
                            // TODO: decide how we can properly allow setup of reverse to work with no looping
                            currentClipIndex = clips.Length - 1;
                            if (loop == Loop.None)
                            {
                                video.Stop();
                                return;
                            }
                        }
                        break;
                    case PlayOrder.Random:
                        currentClipIndex = clips.RandomIndex();
                        break;
                    case PlayOrder.RandomWithoutReplace:
                        if (unreplacedIndices != null && unreplacedIndices.Count == 0 && loop == Loop.None)
                        {
                            unreplacedIndices = null;
                            video.Stop();
                            return;
                        }
                        if (unreplacedIndices == null || unreplacedIndices.Count == 0)
                        {
                            unreplacedIndices = new List<int>();
                            for (var index = 0; index < clips.Length; index++)
                            {
                                unreplacedIndices.Add(index);
                            }
                        }
                        var nextIndiceIndex = unreplacedIndices.RandomIndex();
                        var nextIndex = unreplacedIndices[nextIndiceIndex];
                        unreplacedIndices.RemoveAt(nextIndiceIndex);

                        currentClipIndex = nextIndex;
                        break;
                }
            }

            PlayCurrentClip();
        }

        void PlayCurrentClip()
        {
            video.Stop();

            var clipInfo = clips[currentClipIndex];

            VideoClip nextClip;
            if (clipInfo.clipSet != null && clipInfo.clipSet.Length > 0)
            {
                nextClip = clipInfo.clipSet.RandomElement();
            }
            else
            {
                nextClip = clipInfo.clip;
            }
            video.SetVideoClip(nextClip);
            video.SetPlaybackSpeed(clipInfo.playbackSpeed);

            video.AddNotificationAtTimeFromEnd(HandleFadeInNotification, ((float)video.VideoClip.length) - fadeInAtTimeAfterBeginning);
            video.AddNotificationAtTimeFromEnd(HandleFadeOutNotification, fadeOutAtTimeFromEnd);

            video.Play();

            clipInfo.OnPlay?.InvokeAll(this, EventArgs.Empty);
        }

        void HandleFadeOutNotification(IVideo video)
        {
            FadeOut.InvokeAll(this);
        }

        void HandleFadeInNotification(IVideo video)
        {
            FadeIn.InvokeAll(this);
        }
    }
}