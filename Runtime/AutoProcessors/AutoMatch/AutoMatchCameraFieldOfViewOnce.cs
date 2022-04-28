using System;
using EunomiaUnity;
using UnityEngine;

namespace EunomiaUnity
{
    [Serializable, RequireComponent(typeof(Camera))]
    public class AutoMatchCameraFieldOfViewOnce : AutoProcessor
    {
        [SerializeField]
        private Camera matchTo;

        private bool hasStarted = false;
        public override bool HasStarted => hasStarted;

        private bool hasFinished = false;
        public override bool HasFinished => hasFinished;

        protected override void AutoProcess()
        {
            hasStarted = true;

            if (matchTo == null)
            {
                Debug.LogError("Unable to match field of view: require matchTo Camera reference", this);
                return;
            }

            var myCamera = GetComponent<Camera>();
            if (myCamera == null)
            {
                Debug.LogError("Unable to match field of view: require Camera reference on GameObject", this);
                return;
            }
            myCamera.fieldOfView = matchTo.fieldOfView;

            hasFinished = true;
        }
    }
}