using System;
using Godot;
using Tools;
using Veronenger.Game.Managers;

namespace Veronenger.Game.Controller.Platforms {
    public class SlopeStairsController : DiNode {
        [Inject] public SlopeStairsManager SlopeStairsManager;

        public override void _EnterTree() {
            try {
                var slopeStairs = GetNode<StaticBody2D>("SlopeStairs");
                var cover = GetNode<StaticBody2D>("Cover");
                var upHall = GetNode<Area2D>("UpHall");
                var downHall = GetNode<Area2D>("DownHall");
                var enabler = GetNode<Area2D>("EnablerAndDisabler/Enabler");
                var disabler = GetNode<Area2D>("EnablerAndDisabler/Disabler");

                SlopeStairsManager.ConfigureSlopeStairs(slopeStairs);
                SlopeStairsManager.ConfigureSlopeStairsCover(cover);
                SlopeStairsManager.ConfigureSlopeStairsUp(upHall);
                SlopeStairsManager.ConfigureSlopeStairsDown(downHall);
                SlopeStairsManager.ConfigureSlopeStairsEnabler(enabler);
                SlopeStairsManager.ConfigureSlopeStairsDisabler(disabler);
            } catch (InvalidCastException e) {
                Console.WriteLine("Slope stairs node has a wrong name or a wrong node type");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}