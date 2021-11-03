namespace Veronenger.Game.Tools.Input {
    public abstract class IActionUpdate {
        public bool Enabled = true;
        public abstract bool Update(EventWrapper w);
        public abstract void ClearJustState();
    }
}