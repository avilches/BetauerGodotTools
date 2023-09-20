using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Image;

public class StepRule : BaseRule {
    
    public enum Step {
        UpHalf,  // Top Half
        DownHalf,  // Bottom Half
        RightHalf,  // Right Half
        LeftHalf,  // Left Half
        UpLeftQuarter, // Top Left Quarter
        UpRightQuarter, // Top Right Quarter
        DownLeftQuarter, // Bottom Left Quarter
        DownRightQuarter, // Bottom Right Quarter
    }
    
    private readonly List<Action<TileSetImage>> _actions = new();

    public StepRule(int tileId) {
        TileId = tileId;
        Dependencies = new List<int>();
    }

    public StepRule UpDown(int upTileId, int downTileId) => Do(upTileId, Step.UpHalf).Do(downTileId, Step.DownHalf);
    public StepRule LeftRight(int leftTileId, int rightTileId) => Do(leftTileId, Step.LeftHalf).Do(rightTileId, Step.RightHalf);
    public StepRule Quarters(int leftUpTileId, int rightUpTileId, int downLeftTileId, int downRightTileId) => 
        Do(leftUpTileId, Step.UpLeftQuarter)
            .Do(rightUpTileId, Step.UpRightQuarter)
            .Do(downLeftTileId, Step.DownLeftQuarter)
            .Do(downRightTileId, Step.DownRightQuarter);

    public StepRule Do(int fromTileId, Step step) {
        ((List<int>)Dependencies!).Add(fromTileId);
        if (step == Step.UpHalf) _actions.Add(tileSet => tileSet.CopyTH(fromTileId).PasteTL(tileSet, TileId));
        if (step == Step.DownHalf) _actions.Add(tileSet => tileSet.CopyBH(fromTileId).PasteBL(tileSet, TileId));
        if (step == Step.LeftHalf) _actions.Add(tileSet => tileSet.CopyLH(fromTileId).PasteTL(tileSet, TileId));
        if (step == Step.RightHalf) _actions.Add(tileSet => tileSet.CopyRH(fromTileId).PasteTR(tileSet, TileId));
        
        if (step == Step.UpLeftQuarter) _actions.Add(tileSet => tileSet.CopyTLQ(fromTileId).PasteTL(tileSet, TileId));
        if (step == Step.UpRightQuarter) _actions.Add(tileSet => tileSet.CopyTRQ(fromTileId).PasteTR(tileSet, TileId));
        if (step == Step.DownLeftQuarter) _actions.Add(tileSet => tileSet.CopyBLQ(fromTileId).PasteBL(tileSet, TileId));
        if (step == Step.DownRightQuarter) _actions.Add(tileSet => tileSet.CopyBRQ(fromTileId).PasteBR(tileSet, TileId));
        return this;
    }

    public override void Apply(TileSetImage tileSet) {
        foreach (var action in _actions) {
            action(tileSet);
        }
    }
}