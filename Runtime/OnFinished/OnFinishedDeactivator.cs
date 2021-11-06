// ReSharper disable once CheckNamespace

namespace EunomiaUnity
{
    public class OnFinishedDeactivator : OnFinishedResponder
    {
        protected override void DoResponse()
        {
            this.DoDeactivate();
        }

        public void DoDeactivate()
        {
            this.gameObject.SetActive(false);
        }
    }
}