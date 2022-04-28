using System;
using EunomiaUnity;
using UnityEngine;

namespace EunomiaUnity
{
    [Serializable]
    public class AutoMatchTransformOnce : AutoProcessor
    {
        [SerializeField]
        private Transform matchTo;

        [SerializeField]
        private bool matchPosition = true;
        [SerializeField]
        private bool matchRotation = true;
        [SerializeField]
        private bool matchScale = true;

        private bool hasStarted = false;
        public override bool HasStarted => hasStarted;

        private bool hasFinished = false;
        public override bool HasFinished => hasFinished;

        protected override void AutoProcess()
        {
            hasStarted = true;

            if (matchTo == null)
            {
                Debug.LogError("Unable to match field of view: require matchTo Transform reference", this);
                return;
            }

            var myTransform = GetComponent<Transform>();
            if (myTransform == null)
            {
                Debug.LogError("Unable to match field of view: require Transform reference on GameObject", this);
                return;
            }
            if (matchPosition)
            {
                myTransform.position = matchTo.position;
            }
            if (matchRotation)
            {
                myTransform.rotation = matchTo.rotation;
            }
            if (matchScale)
            {
                if (myTransform.parent != null)
                {
                    myTransform.localScale = new Vector3(
                        matchTo.lossyScale.x / myTransform.parent.lossyScale.x,
                        matchTo.lossyScale.y / myTransform.parent.lossyScale.y,
                        matchTo.lossyScale.z / myTransform.parent.lossyScale.z
                    );
                }
                else
                {
                    Debug.LogError("Unable to match field of view: require parent Transform", this);
                }
            }

            hasFinished = true;
        }
    }
}