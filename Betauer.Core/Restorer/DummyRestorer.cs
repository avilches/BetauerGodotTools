namespace Betauer.Restorer {
    public class DummyRestorer : Restorer {
        public static readonly Restorer Instance = new DummyRestorer();

        private DummyRestorer() {
        }

        protected override void DoSave() {
        }

        protected override void DoRestore() {
        }
    }
}