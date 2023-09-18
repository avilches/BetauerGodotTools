using System;
using System.Collections.Generic;

namespace Betauer.TileSet.Image;

public class StepRule : BaseRule {
    
    public enum Step {
        TH,  // Top Half
        BH,  // Bottom Half
        RH,  // Right Half
        LH,  // Left Half
        TRQ, // Top Right Quarter
        TLQ, // Top Left Quarter
        BRQ, // Bottom Right Quarter
        BLQ, // Bottom Left Quarter
    }
    
    private readonly List<Action<TileSetImage>> _actions = new();

    public StepRule(int tileId) {
        TileId = tileId;
        Dependencies = new List<int>();
    }
    
    public StepRule Do(int fromTileId, Step step) {
        ((List<int>)Dependencies!).Add(fromTileId);
        if (step == Step.TH) _actions.Add(tileSet => tileSet.CopyTH(fromTileId).PasteTL(tileSet, TileId));
        if (step == Step.BH) _actions.Add(tileSet => tileSet.CopyBH(fromTileId).PasteBL(tileSet, TileId));
        if (step == Step.RH) _actions.Add(tileSet => tileSet.CopyRH(fromTileId).PasteTR(tileSet, TileId));
        if (step == Step.LH) _actions.Add(tileSet => tileSet.CopyLH(fromTileId).PasteTL(tileSet, TileId));
        
        if (step == Step.TLQ) _actions.Add(tileSet => tileSet.CopyTLQ(fromTileId).PasteTL(tileSet, TileId));
        if (step == Step.TRQ) _actions.Add(tileSet => tileSet.CopyTRQ(fromTileId).PasteTR(tileSet, TileId));
        if (step == Step.BLQ) _actions.Add(tileSet => tileSet.CopyBLQ(fromTileId).PasteBL(tileSet, TileId));
        if (step == Step.BRQ) _actions.Add(tileSet => tileSet.CopyBRQ(fromTileId).PasteBR(tileSet, TileId));
        return this;
    }

    public override void Apply(TileSetImage tileSet) {
        foreach (var action in _actions) {
            action(tileSet);
        }
    }
}