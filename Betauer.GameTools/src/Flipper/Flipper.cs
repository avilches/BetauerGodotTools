namespace Betauer.Flipper;

public abstract class Flipper : IFlipper {
    private bool? _isFacingRight = null;

    public abstract bool LoadIsFacingRight();
    public abstract void SetFacingRight(bool right);

    public void Flip() => IsFacingRight = !IsFacingRight;

    public void Flip(float xInput) {
        if (xInput != 0) IsFacingRight = xInput > 0;
    }

    public bool IsFacingRight {
        get {
            if (!_isFacingRight.HasValue) {
                _isFacingRight = LoadIsFacingRight();
                SetFacingRight(_isFacingRight.Value);
            }
            return _isFacingRight.Value;
        }
        set {
            if (_isFacingRight == value) return;
            _isFacingRight = value;
            SetFacingRight(value);
        }
    }
}