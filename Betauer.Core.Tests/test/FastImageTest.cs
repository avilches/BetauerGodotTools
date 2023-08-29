using Betauer.Core.Image;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class FastImageTest {

    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test1() {
        var fast = new FastImage(100, 100);
        Assert.That(fast.Format, Is.EqualTo(FastImage.DefaultFormat));
        //
        var yellow = Colors.Yellow;
        yellow.A = 0.5f;
        var green = Colors.DarkGreen;
        green.A = 0.5f;
        var gray = Colors.Black;
        gray.A = 0.2f;
        fast.DrawCircle(6, 6, 6, Colors.Red);
        fast.DrawCircle(20, 20, 20, Colors.Blue);
        fast.DrawCircle(35, 14, 10, Colors.Green, true);
        fast.SetPixel(35, 10, Colors.Azure);
        fast.DrawRectangle(16, 16, 60, 15, Colors.Red);
        fast.DrawRectangle(30, 0, 20, 60, yellow);
        fast.DrawRectangle(45, 24, 10, 16, green, true);
        fast.DrawRectangle(20, 55, 70, 40, green, true);
        fast.Fill(Color.FromHtml("00000033"));
        fast.Flush();
        
        var fastTree = new FastImage("test-resources/trees-16x16.png");
        
        fast.BlitRect(fastTree, new Rect2I(16, 32, 16, 16), new Vector2I(50, 60));
        fast.BlendRect(fastTree, new Rect2I(16, 32, 16, 16), new Vector2I(66, 76));
        fast.DrawLine(0, 0, 100, 100, gray);
        fast.DrawLine(0, 40, 100, 100, gray);
        fast.Flush();
        fast.Image.SavePng("test1.png");
    }

    
    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test3() {
        var fast = new FastImage(256, 256, false, Godot.Image.Format.Rgba8);
        fast.Fill(Color.FromHtml("00000033"));
        var fastTree = new FastImage("test-resources/trees-16x16.png", Godot.Image.Format.Rgba8);
        
        var newImage = new FastImage(256, 256, false, Godot.Image.Format.Rgba8);
        newImage.Fill(Colors.White);
        fast.BlitRectMask(newImage, fastTree, new Rect2I(0,0, 100, 100), new Vector2I(0,0 ));
        
        fast.Image.SavePng("test2.png");
    }
    
}