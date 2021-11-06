using System;
using UnityEngine;
using UnityEngine.Video;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public static class VideoPlayerExtensions
    {
        public static RenderTexture CurrentRenderTexture(this VideoPlayer videoPlayer)
        {
            if (videoPlayer == null)
            {
                throw new ArgumentNullException(nameof(videoPlayer));
            }

            return (RenderTexture)videoPlayer.texture;
        }
    }
}