using Godot;
using Tools;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Platforms {
    public class SlopeStairsController : DiNode {
        [Inject] public SlopeStairsManager SlopeStairsManager;

        [OnReady("SlopeStairs")] StaticBody2D slopeStairs;
        [OnReady("Cover")] StaticBody2D cover;
        [OnReady("UpHall")] Area2D upHall;
        [OnReady("DownHall")] Area2D downHall;
        [OnReady("EnablerAndDisabler/Enabler")] Area2D enabler;
        [OnReady("EnablerAndDisabler/Disabler")] Area2D disabler;

        public override void Ready() {
            SlopeStairsManager.ConfigureSlopeStairs(slopeStairs);
            SlopeStairsManager.ConfigureSlopeStairsCover(cover);
            SlopeStairsManager.ConfigureSlopeStairsUp(upHall);
            SlopeStairsManager.ConfigureSlopeStairsDown(downHall);
            SlopeStairsManager.ConfigureSlopeStairsEnabler(enabler);
            SlopeStairsManager.ConfigureSlopeStairsDisabler(disabler);
        }
    }
}