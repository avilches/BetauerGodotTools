using System;
using Godot;
using Veronenger.Game.Managers.Autoload;

namespace Veronenger.Game.Controller.Platforms {
    public class SlopeStairsController : Node {
        public override void _EnterTree() {
            try {
                var slopeStairs = GetNode<StaticBody2D>("SlopeStairs");
                var cover = GetNode<StaticBody2D>("Cover");
                var upHall = GetNode<Area2D>("UpHall");
                var downHall = GetNode<Area2D>("DownHall");
                var enabler = GetNode<Area2D>("EnablerAndDisabler/Enabler");
                var disabler = GetNode<Area2D>("EnablerAndDisabler/Disabler");

                GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairs(slopeStairs);
                GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairsCover(cover);
                GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairsUp(upHall);
                GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairsDown(downHall);
                GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairsEnabler(enabler);
                GameManager.Instance.SlopeStairsManager.ConfigureSlopeStairsDisabler(disabler);
            } catch (InvalidCastException e) {
                Console.WriteLine("Slope stairs node has a wrong name or a wrong node type");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}