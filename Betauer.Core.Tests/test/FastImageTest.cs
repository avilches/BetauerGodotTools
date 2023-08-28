using Betauer.Core.Image;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class FastImageTest {

    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test() {
        var fast = new FastImage(100, 100);

        var yellow = Colors.Yellow;
        yellow.A = 0.5f;
        var green = Colors.DarkGreen;
        green.A = 0.5f;
        fast.DrawCircle(6, 6, 6, Colors.Red);
        fast.DrawCircle(20, 20, 20, Colors.Blue);
        fast.DrawCircle(35, 14, 10, Colors.Green, true);
        fast.SetPixel(35, 10, Colors.Azure);
        fast.DrawRectangle(16, 16, 60, 15, Colors.Red);
        fast.DrawRectangle(30, 0, 20, 60, yellow);
        fast.DrawRectangle(45, 24, 10, 16, green, true);
        fast.DrawLine(0, 0, 100, 100, Colors.Aquamarine);
        fast.Fill(Color.FromHtml("00000033"));
        fast.Flush();
        fast.Image.SavePng("test1.png");
    }
    
}