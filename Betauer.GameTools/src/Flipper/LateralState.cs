using System;
using Betauer.Core;
using Godot;

namespace Betauer.Flipper;

public class LateralState : IFlipper {
    private readonly IFlipper _flipper;
    private readonly Func<Vector2> _lookRightDirection; 
    private readonly Func<Vector2> _globalPosition;

    public LateralState(IFlipper flipper, Func<Vector2> lookRightDirection, Func<Vector2> globalPosition) {
        _flipper = flipper;
        _lookRightDirection = lookRightDirection;
        _globalPosition = globalPosition;
    }

    public void Flip() => _flipper.Flip();

    public void Flip(float xInput) => _flipper.Flip(xInput);

    public bool IsFacingRight {
        get => _flipper.IsFacingRight;
        set => _flipper.IsFacingRight = value;
    }

    public Vector2 FacingDirection => IsFacingRight ? _lookRightDirection() : -_lookRightDirection();
    
    /// <summary>
    /// Return true if the current body is to the right of the parameter 
    /// </summary>
    /// <param name="globalPosition"></param>
    /// <returns></returns>
    public bool IsToTheRightOf(Vector2 globalPosition) => _lookRightDirection().IsOppositeDirection(_globalPosition().DirectionTo(globalPosition));
    
    /*
     *  IsToTheRightOf | IsFacingRight | 
     *  true           | true          |   globalPosition  -   Body:)
     *  true           | false         |   globalPosition  - (:Body
     *  false          | true          |   Body:)  -  globalPosition
     *  false          | false         | (:Body    -  globalPosition
     */
    public bool IsFacingTo(Vector2 globalPosition) => IsToTheRightOf(globalPosition) != IsFacingRight;

    public int FacingRight => IsFacingRight ? 1 : -1; 

    public void FaceTo(Node2D node2D) => FaceTo(node2D.GlobalPosition);
    public void FaceOppositeTo(Node2D node2D) => FaceOppositeTo(node2D.GlobalPosition);
    public bool IsFacingTo(Node2D node2D) => IsFacingTo(node2D.GlobalPosition);

    public void FaceTo(Vector2 globalPosition) {
        if (!IsFacingTo(globalPosition)) Flip();
    }

    public void FaceOppositeTo(Vector2 globalPosition) {
        if (IsFacingTo(globalPosition)) Flip();
    }


}