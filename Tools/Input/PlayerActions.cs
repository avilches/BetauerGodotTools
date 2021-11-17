namespace Tools.Input {
    public abstract class PlayerActions {
        public bool IsUsingKeyboard = true;
        protected readonly ActionInputList ActionInputList;

        public PlayerActions(int deviceId) {
            ActionInputList = new ActionInputList(this, deviceId);
        }

        public bool Update(EventWrapper w) {
            return ActionInputList.Update(w);
        }

        public void ClearJustStates() {
            ActionInputList.ClearJustState();
        }
    }
}