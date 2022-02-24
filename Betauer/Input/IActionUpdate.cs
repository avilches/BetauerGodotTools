namespace Betauer.Input {
    public abstract class IActionUpdate {
        public string Name;
        public bool Enabled = true;
        public abstract bool Update(EventWrapper w);
        public abstract void ClearJustPressedState();
        public abstract void ClearPressedState();

        protected IActionUpdate(string name) {
            Name = name;
        }
    }
}