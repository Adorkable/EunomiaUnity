using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Video;

namespace EunomiaUnity
{
    public interface IVideo
    {
        void Play();
        void Prepare();
        void Pause();
        void Stop();

        bool IsPlaying { get; }
        bool IsLooping { get; set; }
        bool PlayOnAwake { get; set; }

        bool IsAssignedContent { get; }
        VideoSource ContentSource { get; set; }
        VideoClip VideoClip { get; set; }
        string Url { get; set; }

        /// <summary>
        /// Side effects: 
        /// * If clip is value ContentSource will be set to VideoClip.
        /// * If clip is value will Prepare video player.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns>Task that completes when video player is prepared</returns>
        UniTask SetVideoClip(VideoClip clip);
        /// <summary>
        /// Side effects: 
        /// * If video url is value ContentSource will be set to Url.
        /// * If video url is value will Prepare video player.
        /// </summary>
        /// <param name="clip"></param>
        /// <returns>Task that completes when video player is prepared</returns>
        UniTask SetUrl(string url);
        bool IsPrepared { get; }

        double Duration { get; }
        double Time { get; set; }
        long Frame { get; set; }

        Vector2Int Size { get; }

        VideoRenderMode RenderMode { get; set; }

        RenderTexture TargetTexture { get; set; }

        event EventHandler OnLoopPointReached;
        event EventHandler<string> OnErrorReceived;

        IVideoNotification AddNotificationAtPercent(Action<IVideo> perform, float atPercent);
        IVideoNotification AddNotificationAtTimeFromEnd(Action<IVideo> perform, float atTimeFromEnd);
        void CancelNotification(IVideoNotification notification);
    }

    public interface IVideoNotification
    {
        Action<IVideo> perform { get; }
    }
}