using System;
using Betauer.Core.Image;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class FastImageTest {

    [TestRunner.Test]
    // [TestRunner.Ignore("Just create images")]
    [TestRunner.Only]
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
        fast.FillCircle(35, 14, 10, Colors.Green);
        fast.SetPixel(35, 10, Colors.Azure);
        fast.DrawRect(16, 16, 60, 15, Colors.Red);
        fast.DrawRect(30, 0, 20, 60, yellow);
        fast.FillRect(45, 24, 10, 16, green, true);
        fast.FillRect(20, 55, 70, 40, green, true);
        fast.Fill(Color.FromHtml("22224433"));
        fast.Flush();
        
        var fastTree = new FastImage("test-resources/trees-16x16.png");
        
        fast.BlitRect(fastTree, new Rect2I(16, 32, 16, 16), new Vector2I(50, 60));
        fast.BlendRect(fastTree, new Rect2I(16, 32, 16, 16), new Vector2I(66, 76));
        fast.DrawLine(10, 0, 10, 100, Colors.Fuchsia);
        fast.DrawLine(0, 10, 100, 10, Colors.Fuchsia);
        fast.DrawLine(26, 60, 26, 60, Colors.Fuchsia);
        fast.DrawLine(0, 0, 100, 100, gray);
        fast.DrawLine(1, 0, 101, 100, gray);
        fast.DrawLine(0, 40, 100, 100, gray);
        fast.Flush();
        fast.Image.SavePng("test1.png");
    }

    
    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test2() {
        var fast = new FastImage(256, 256, false, Godot.Image.Format.Rgba8);
        fast.Fill(Color.FromHtml("00000033"));
        var fastTree = new FastImage("test-resources/trees-16x16.png", Godot.Image.Format.Rgba8);
        
        var newImage = new FastImage(256, 256, false, Godot.Image.Format.Rgba8);
        newImage.Fill(Colors.White);
        fast.BlitRectMask(newImage, fastTree, new Rect2I(0,0, 100, 100), new Vector2I(0,0 ));
        
        fast.Image.SavePng("test2.png");
    }
    
    [TestRunner.Test]
    [TestRunner.Only]
    public void Test3() {
        var compo = new LayerImageComposition(3, 512, 512);
        
        var textures = new FastImage("test-resources/GrasslandsTextures.png", Godot.Image.Format.Rgba8);
        compo.SetLayer(0, textures.Extract( null, 0, 0, 80, 80), true);
        compo.SetLayer(1, textures.Extract( null, 80, 0, 80, 80).ExpandTiled(512, 512), true);

        var thick = 24f;
        var halfThick = thick/2f;
        for (int i = 0; i < thick; i++) {
            Draw.Line(0, 10+i, 512, 64+i, (x, y) => {
                // compo.GetLayer(1).Image.SetPixel(x, y, new Color(1, 1, 1, 0.5f), false);
                compo.GetLayer(1).Image.SetAlpha(x, y, Math.Abs(halfThick - i) / halfThick);

            });
        }
        
        compo.Export().Image.SavePng("test3.png");
    }
    
}