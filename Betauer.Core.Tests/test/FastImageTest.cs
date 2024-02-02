using System;
using System.Diagnostics;
using Betauer.Core.Image;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestRunner.Test]
public class FastImageTest {

    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test1() {
        var fast = new FastImage(150, 100);
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
        fast.DrawLine(10, 0, 10, 100, 1, Colors.Fuchsia);
        fast.DrawLine(0, 10, 100, 10, 1, Colors.Fuchsia);
        fast.DrawLine(26, 60, 26, 60, 1, Colors.Fuchsia);
        fast.DrawLine(0, 0, 100, 100, 1, gray);
        fast.DrawLine(1, 0, 101, 100, 1, gray);
        fast.DrawLine(0, 40, 100, 100,1,  gray);

        fast.DrawLine(100, 0, 150, 20, 3, Colors.Red, true);

        fast.FillCircle(45,  30, 0, Colors.Green);
        fast.FillCircle(48,  30, 1, Colors.Green);
        fast.FillCircle(54,  30, 2, Colors.Green);
        fast.FillCircle(61,  30, 3, Colors.Green);
        fast.FillCircle(70,  30, 4, Colors.Green);
        fast.FillCircle(81,  30, 5, Colors.Green);
        fast.FillCircle(94,  30, 6, Colors.Green);
        fast.FillCircle(110, 30, 7, Colors.Green);
        fast.FillCircle(128, 30, 8, Colors.Green);
        
        fast.DrawCircle(45,  70, 0, Colors.Green);
        fast.DrawCircle(48,  70, 1, Colors.Green);
        fast.DrawCircle(54,  70, 2, Colors.Green);
        fast.DrawCircle(61,  70, 3, Colors.Green);
        fast.DrawCircle(70,  70, 4, Colors.Green);
        fast.DrawCircle(81,  70, 5, Colors.Green);
        fast.DrawCircle(94,  70, 6, Colors.Green);
        fast.DrawCircle(110, 70, 7, Colors.Green);
        fast.DrawCircle(128, 70, 8, Colors.Green);

        fast.GradientCircle(30, 30, 10, Colors.Red);

        fast.DrawLineAntialiasing(100, 100, 150, 55, 1, Colors.Wheat);
        fast.DrawLineAntialiasing(100, 100, 150, 60, 2, Colors.Green);
        fast.DrawLineAntialiasing(100, 100, 150, 70, 3, Colors.BlueViolet);
        fast.DrawLineAntialiasing(100, 100, 150, 80, 4, Colors.Fuchsia);

        fast.Flush();

        fast.Image.SavePng("test1.png");
    }
    
    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void TestEllipses() {
        var fast = new FastImage(300, 300);
        fast.Fill(Colors.White);
        
        fast.FillEllipse(45,  45, 0, 0, Colors.Green);
        fast.FillEllipse(48,  45, 1, 2, Colors.Green);
        fast.FillEllipse(54,  45, 2, 3, Colors.Green);
        fast.FillEllipse(61,  45, 3, 4, Colors.Green);
        fast.FillEllipse(70,  45, 4, 5, Colors.Green);
        fast.FillEllipse(81,  45, 5, 8, Colors.Green);
        fast.FillEllipse(94,  45, 6, 12, Colors.Green);
        fast.FillEllipse(110, 45, 7, 6, Colors.Green);
        fast.FillEllipse(128, 45, 8, 4, Colors.Green);

        fast.FillEllipse(145,  45, 0, 0, Colors.Green);
        fast.FillEllipse(148,  45, 1, 2, Colors.Green);
        fast.FillEllipse(154,  45, 2, 3, Colors.Green);
        fast.FillEllipse(161,  45, 3, 4, Colors.Green);
        fast.FillEllipse(170,  45, 4, 5, Colors.Green);
        fast.FillEllipse(181,  45, 5, 8, Colors.Green);
        fast.FillEllipse(194,  45, 6, 12, Colors.Green);
        fast.FillEllipse(210, 45, 7, 6, Colors.Green);
        fast.FillEllipse(228, 45, 8, 4, Colors.Green);
        
        fast.DrawEllipse(45,  75, 0, 0, Colors.Red);
        fast.DrawEllipse(48,  75, 1, 2, Colors.Red);
        fast.DrawEllipse(54,  75, 2, 3, Colors.Red);
        fast.DrawEllipse(61,  75, 3, 4, Colors.Red);
        fast.DrawEllipse(70,  75, 4, 5, Colors.Red);
        fast.DrawEllipse(81,  75, 5, 8, Colors.Red);
        fast.DrawEllipse(94,  75, 6, 12, Colors.Red);
        fast.DrawEllipse(110, 75, 7, 6, Colors.Red);
        fast.DrawEllipse(128, 75, 8, 4, Colors.Red);
        
        fast.GradientEllipse(145,  75, 0, 0, Colors.Red);
        fast.GradientEllipse(148,  75, 1, 2, Colors.Red);
        fast.GradientEllipse(154,  75, 2, 3, Colors.Red);
        fast.GradientEllipse(161,  75, 3, 4, Colors.Red);
        fast.GradientEllipse(170,  75, 4, 5, Colors.Red);
        fast.GradientEllipse(181,  75, 5, 8, Colors.Red);
        fast.GradientEllipse(194,  75, 6, 12, Colors.Red);
        fast.GradientEllipse(210,  75, 7, 6, Colors.Red);
        fast.GradientEllipse(228,  75, 8, 4, Colors.Red);

        fast.Flush();
        fast.Image.SavePng("test-ellipse.png");
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
    [TestRunner.Ignore("Just create images")]
    public void Test3() {
        var compo = new LayerImageComposition(3, 512, 512);
        
        var textures = new FastImage("test-resources/GrasslandsTextures.png", Godot.Image.Format.Rgba8);
        compo.SetLayer(0, textures.Extract( null, 0, 0, 80, 80), true);
        compo.SetLayer(1, textures.Extract( null, 80, 0, 80, 80).ExpandTiled(512, 512), true);

        var layer1 = compo.GetLayer(1).Image;
        layer1.DrawLineAntialiasing(0, 10, 512, 64, 24, Colors.Green);

        var step = 0;
        var circleStep = 0;
        Draw.Line1Width(0, 80, 512, 190, (x, y) => {
            if (step++ % 30 == 0) {
                Draw.GradientCircle(x, y, 20 + ((circleStep++ % 5))*2, (x, y, g) => {
                    if (x < 0 || y < 0 || x >= layer1.Width || y >= layer1.Height) return;
                    layer1.SetAlpha(x, y, Math.Min(layer1.GetAlpha(x, y), g));
                    // layer1.SetPixel(x, y, Colors.Azure, false);
                });
                // layer1.FillCircle(x, y, 20 + ((circleStep++ % 5))*2, Colors.Aqua);
            }
        });
        
        Draw.Line(0, 512, 80, -10, 12, (x, y) => {
            layer1.SetPixel(x,y,Colors.Green);
        });
        
        compo.Export().Image.SavePng("test3.png");
    }
    
}