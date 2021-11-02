using System;
using Godot;

namespace Game.Tools.Platforms {
    public class SlopeStairs : Node {
        public override void _EnterTree() {
            try {
                var slopeStairs = GetNode<StaticBody2D>("SlopeStairs");
                var cover = GetNode<StaticBody2D>("Cover");
                var upHall = GetNode<Area2D>("UpHall");
                var downHall = GetNode<Area2D>("DownHall");
                var enabler = GetNode<Area2D>("EnablerAndDisabler/Enabler");
                var disabler = GetNode<Area2D>("EnablerAndDisabler/Disabler");

                GameManager.Instance.PlatformManager.ConfigureSlopeStairs(slopeStairs);
                GameManager.Instance.PlatformManager.ConfigureSlopeStairsCover(cover);
                GameManager.Instance.PlatformManager.AddArea2DSlopeStairsUp(upHall);
                GameManager.Instance.PlatformManager.AddArea2DSlopeStairsDown(downHall);
                GameManager.Instance.PlatformManager.AddArea2DSlopeStairsEnabler(enabler);
                GameManager.Instance.PlatformManager.AddArea2DSlopeStairsDisabler(disabler);
            } catch (InvalidCastException e) {
                Console.WriteLine("Slope stairs node has a wrong name or a wrong node type");
                Console.WriteLine(e);
                throw;
            }
        }
    }
}