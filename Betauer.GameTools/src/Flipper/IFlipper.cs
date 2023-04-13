namespace Betauer.Flipper;

public interface IFlipper {
    public void Flip();
    public void Flip(float xInput);
    public bool IsFacingRight { get; set; }
}