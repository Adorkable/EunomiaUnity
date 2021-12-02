using System;
using System.Collections;
using System.Collections.Generic;
using Eunomia;
using EunomiaUnity;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Video;

namespace Tests
{
    [AddComponentMenu("Tests/Testers/Overlapping Videos Controller")]
    [Serializable]
    public class OverlappingVideosControllerTester : MonoBehaviour
    {
        [SerializeField]
        private OverlappingVideosController overlappingVideosController;

        [SerializeField]
        private VideoClip[] videoClips;

        public void Start()
        {
            if (overlappingVideosController == null)
            {
                // overlappingVideosController = this.Requi
            }

            SetNextVideo();
            SetNextVideo();
        }

        void SetNextVideo()
        {
            overlappingVideosController.SetNextVideoClip(videoClips.RandomElement());
        }
    }
}