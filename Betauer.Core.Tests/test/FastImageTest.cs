using System;
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
        var fast = new FastImage().Create(150, 100);
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
        
        var fastTree = new FastImage().LoadResource("test-resources/trees-16x16.png");
        
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
        var fast = new FastImage().Create(300, 300);
        fast.Fill(Colors.DarkBlue);
        
        fast.DrawEllipse(45,  45, 0, 0, Colors.Green);
        fast.DrawEllipse(48,  45, 1, 2, Colors.Green);
        fast.DrawEllipse(54,  45, 2, 3, Colors.Green);
        fast.DrawEllipse(61,  45, 3, 4, Colors.Green);
        fast.DrawEllipse(70,  45, 4, 5, Colors.Green);
        fast.DrawEllipse(81,  45, 5, 8, Colors.Green);
        fast.DrawEllipse(94,  45, 6, 12, Colors.Green);
        fast.DrawEllipse(110, 45, 7, 6, Colors.Green);
        fast.DrawEllipse(128, 45, 8, 4, Colors.Green);

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

        
        fast.DrawEllipseRotated(45,  105, 0, 0,  0, Colors.Blue);
        fast.DrawEllipseRotated(48,  105, 1, 2,  0, Colors.Blue);
        fast.DrawEllipseRotated(54,  105, 2, 3,  0, Colors.Blue);
        fast.DrawEllipseRotated(61,  105, 3, 4,  0, Colors.Blue);
        fast.DrawEllipseRotated(70,  105, 4, 5,  0, Colors.Blue);
        fast.DrawEllipseRotated(81,  105, 5, 8,  0, Colors.Blue);
        fast.DrawEllipseRotated(94,  105, 6, 12, 0, Colors.Blue);
        fast.DrawEllipseRotated(110, 105, 7, 6,  0, Colors.Blue);
        fast.DrawEllipseRotated(128, 105, 8, 4,  0, Colors.Blue);

        fast.DrawEllipseRotated(145+5*0, 105, 0, 0,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(148+5*1, 105, 1, 2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(154+5*2, 105, 2, 3,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(161+5*3, 105, 3, 4,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(170+5*4, 105, 4, 5,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(181+5*5, 105, 5, 8,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(194+5*6, 105, 6, 12, Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(210+5*7, 105, 7, 6,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(228+5*8, 105, 8, 4,  Mathf.DegToRad(45f), Colors.Aquamarine);


        fast.DrawEllipseRotated( 45+10*0, 140, 0*2,  0*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated( 48+10*1, 140, 1*2,  2*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated( 54+10*2, 140, 2*2,  3*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated( 61+10*3, 140, 3*2,  4*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated( 70+10*4, 140, 4*2,  5*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated( 81+10*5, 140, 5*2,  8*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated( 94+10*6, 140, 6*2, 12*2, Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(110+10*7, 140, 7*2,  6*2,  Mathf.DegToRad(45f), Colors.Aquamarine);
        fast.DrawEllipseRotated(128+10*8, 140, 8*2,  4*2,  Mathf.DegToRad(45f), Colors.Aquamarine);

        fast.FillEllipseRotated( 45+10*0, 200, 0*2,  0*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated( 48+10*1, 200, 1*2,  2*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated( 54+10*2, 200, 2*2,  3*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated( 61+10*3, 200, 3*2,  4*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated( 70+10*4, 200, 4*2,  5*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated( 81+10*5, 200, 5*2,  8*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated( 94+10*6, 200, 6*2, 12*2, Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated(110+10*7, 200, 7*2,  6*2,  Mathf.DegToRad(45f), Colors.Salmon);
        fast.FillEllipseRotated(128+10*8, 200, 8*2,  4*2,  Mathf.DegToRad(45f), Colors.Salmon);

        fast.GradientEllipseRotated( 45+10*0, 260, 0*2,  0*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated( 48+10*1, 260, 1*2,  2*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated( 54+10*2, 260, 2*2,  3*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated( 61+10*3, 260, 3*2,  4*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated( 70+10*4, 260, 4*2,  5*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated( 81+10*5, 260, 5*2,  8*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated( 94+10*6, 260, 6*2, 12*2, Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated(110+10*7, 260, 7*2,  6*2,  Mathf.DegToRad(45f), Colors.Lime);
        fast.GradientEllipseRotated(128+10*8, 260, 8*2,  4*2,  Mathf.DegToRad(45f), Colors.Lime);

        fast.Flush();
        fast.Image.SavePng("test-ellipse.png");
    }
    
    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test2() {
        var fast = new FastImage().Create(256, 256, false, Godot.Image.Format.Rgba8);
        fast.Fill(Color.FromHtml("00000033"));
        var fastTree = new FastImage().LoadResource("test-resources/trees-16x16.png", Godot.Image.Format.Rgba8);
        
        var newImage = new FastImage().Create(256, 256, false, Godot.Image.Format.Rgba8);
        newImage.Fill(Colors.White);
        fast.BlitRectMask(newImage, fastTree, new Rect2I(0,0, 100, 100), new Vector2I(0,0 ));
        
        fast.Image.SavePng("test2.png");
    }
    
    [TestRunner.Test]
    [TestRunner.Ignore("Just create images")]
    public void Test3() {
        var compo = new LayerImageComposition(3, 512, 512);
        
        var textures = new FastImage().LoadResource("test-resources/GrasslandsTextures.png", Godot.Image.Format.Rgba8);
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