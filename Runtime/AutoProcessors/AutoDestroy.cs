using System;
using EunomiaUnity;
using UnityEngine;

namespace EunomiaUnity
{
    [Serializable]
    public class AutoDestroy : AutoProcessor
    {
        private bool hasStarted = false;
        public override bool HasStarted => hasStarted;

        private bool hasFinished = false;
        public override bool HasFinished => hasFinished;

        protected override void AutoProcess()
        {
            hasStarted = true;

            transform.parent = null;

            Destroy(gameObject);

            hasFinished = true;
        }
    }
}