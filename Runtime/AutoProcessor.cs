using System;
using System.Collections;
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
            AfterProcessorFinished,
            None
        }
        [SerializeField]
        private AutoProcessTime autoProcessTime = AutoProcessTime.Start;

        [SerializeField]
        private Processor afterProcessorFinished;

        protected abstract void AutoProcess();

        protected virtual void Awake()
        {
            if (autoProcessTime == AutoProcessTime.Awake)
            {
                AutoProcess();
            }
            else if (autoProcessTime == AutoProcessTime.AfterProcessorFinished)
            {
                StartCoroutine(AfterProcessorFinishedCoroutine());
            }
        }

        protected virtual void Start()
        {
            if (autoProcessTime == AutoProcessTime.Start)
            {
                AutoProcess();
            }
        }

        protected virtual void OnDestroy()
        {
            if (autoProcessTime == AutoProcessTime.OnDestroy)
            {
                AutoProcess();
            }
        }

        IEnumerator AfterProcessorFinishedCoroutine()
        {
            yield return new WaitUntil(() => afterProcessorFinished.HasFinished);
            AutoProcess();
        }
    }
}
