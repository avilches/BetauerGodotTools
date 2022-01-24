namespace Betauer.Input {
    public abstract class IActionUpdate {
        public string Name;
        public bool Enabled = true;
        public abstract bool Update(EventWrapper w);
        public abstract void ClearJustState();
        public abstract void ClearState();

        protected IActionUpdate(string name) {
            Name = name;
        }

    }
}