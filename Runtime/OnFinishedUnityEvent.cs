using UnityEngine;
using UnityEngine.Events;

namespace EunomiaUnity
{
    public class OnFinishedUnityEvent : OnFinishedResponder
    {
        [SerializeField]
        private UnityEvent unityEvent;

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
