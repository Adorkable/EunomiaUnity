using System;

// ReSharper disable once CheckNamespace
namespace EunomiaUnity
{
    public class OnFinishedEvent : OnFinishedResponder
    {
        public event EventHandler OnFinished;

        protected override void DoResponse()
        {
            this.DoInvokeEvent();
        }

        public void DoInvokeEvent()
        {
            OnFinished.Invoke(this, EventArgs.Empty);
        }
    }
}