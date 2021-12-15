using System;
using UnityEngine;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public abstract class AutoProcessor : Processor
    {
        [Serializable]
        public enum AutoProcessTime
        {
            Awake,
            Start,
            OnDestroy,
            None
        }
        [SerializeField]
        private AutoProcessTime autoProcessTime = AutoProcessTime.Start;

        protected abstract void AutoProcess();

        void Awake()
        {
            if (autoProcessTime == AutoProcessTime.Awake)
            {
                AutoProcess();
            }
        }

        void Start()
        {
            if (autoProcessTime == AutoProcessTime.Start)
            {
                AutoProcess();
            }
        }

        void OnDestroy()
        {
            if (autoProcessTime == AutoProcessTime.OnDestroy)
            {
                AutoProcess();
            }
        }
    }
}
