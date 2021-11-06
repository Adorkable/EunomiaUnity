using UnityEngine;
using UnityEngine.Events;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class OnFinishedUnityEvent : OnFinishedResponder
    {
        [SerializeField] private UnityEvent unityEvent;

        protected override void DoResponse()
        {
            this.DoInvokeEvent();
        }

        public void DoInvokeEvent()
        {
            unityEvent.Invoke();
        }
    }
}