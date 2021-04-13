namespace EunomiaUnity {
    public class OnFinishedDestroyer : OnFinishedResponder {
        protected override void DoResponse() {
            this.DoDestroy();
        }

        public void DoDestroy() {
            Destroy(this.gameObject);
        }
    }

}
