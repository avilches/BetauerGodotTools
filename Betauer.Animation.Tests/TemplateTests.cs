using Godot;
using NUnit.Framework;
using Betauer.TestRunner;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class TemplateTests : NodeTest {
        [SetUp]
        public void SetUp() {
            Engine.TimeScale = 10;
        }

        [TearDown]
        public void TearDown() {
            Engine.TimeScale = 1;
        }

        [Test]
        public void TemplateNamesCheck() {
            Assert.That(Template.Bounce, Is.EqualTo(Template.Get("bOunce")));
            Assert.That(Template.Flash, Is.EqualTo(Template.Get("fLash")));
            Assert.That(Template.HeadShake, Is.EqualTo(Template.Get("hEadshake")));
            Assert.That(Template.HeartBeat, Is.EqualTo(Template.Get("hEartbeat")));
            Assert.That(Template.Jello, Is.EqualTo(Template.Get("jEllo")));
            Assert.That(Template.Pulse, Is.EqualTo(Template.Get("pUlse")));
            Assert.That(Template.RubberBand, Is.EqualTo(Template.Get("rUbberband")));
            Assert.That(Template.Shake, Is.EqualTo(Template.Get("sHake")));
            Assert.That(Template.ShakeX, Is.EqualTo(Template.Get("sHakex")));
            Assert.That(Template.ShakeY, Is.EqualTo(Template.Get("sHakey")));
            Assert.That(Template.Swing, Is.EqualTo(Template.Get("sWing")));
            Assert.That(Template.Tada, Is.EqualTo(Template.Get("tAda")));
            Assert.That(Template.Wobble, Is.EqualTo(Template.Get("wObble")));
            Assert.That(Template.BackInUp, Is.EqualTo(Template.Get("bAckInUp")));
            Assert.That(Template.BackInDown, Is.EqualTo(Template.Get("bAckINdowN")));
            Assert.That(Template.BackInLeft, Is.EqualTo(Template.Get("bAckinLeft")));
            Assert.That(Template.BackInRight, Is.EqualTo(Template.Get("bAckinRight")));
            Assert.That(Template.BackOutUp, Is.EqualTo(Template.Get("bAckOUTup")));
            Assert.That(Template.BackOutDown, Is.EqualTo(Template.Get("bAckOUTdowN")));
            Assert.That(Template.BackOutLeft, Is.EqualTo(Template.Get("bAckOUTLeft")));
            Assert.That(Template.BackOutRight, Is.EqualTo(Template.Get("bAckOUTRight")));

            Assert.That(Template.BackInUpFactory.Get(100f), Is.EqualTo(Template.Get("bAckInUp", 100f)));
            Assert.That(Template.BackInDownFactory.Get(100f), Is.EqualTo(Template.Get("bAckINdowN", 100f)));
            Assert.That(Template.BackInLeftFactory.Get(100f), Is.EqualTo(Template.Get("bAckinLeft", 100f)));
            Assert.That(Template.BackInRightFactory.Get(100f), Is.EqualTo(Template.Get("bAckinRight", 100f)));

            Assert.That(Template.BackOutUpFactory.Get(100f), Is.EqualTo(Template.Get("bAckOUTup", 100f)));
            Assert.That(Template.BackOutDownFactory.Get(100f), Is.EqualTo(Template.Get("bAckOUTdowN", 100f)));
            Assert.That(Template.BackOutLeftFactory.Get(100f), Is.EqualTo(Template.Get("bAckOUTLeft", 100f)));
            Assert.That(Template.BackOutRightFactory.Get(100f), Is.EqualTo(Template.Get("bAckOUTRight", 100f)));

            Assert.That(Template.BounceInFactory.Get(), Is.EqualTo(Template.Get("BounceIN")));
            Assert.That(Template.BounceInUpFactory.Get(100f), Is.EqualTo(Template.Get("BounceINup", 100f)));
            Assert.That(Template.BounceInDownFactory.Get(100f), Is.EqualTo(Template.Get("BounceINdowN", 100f)));
            Assert.That(Template.BounceInLeftFactory.Get(100f), Is.EqualTo(Template.Get("BounceINLeft", 100f)));
            Assert.That(Template.BounceInRightFactory.Get(100f), Is.EqualTo(Template.Get("BounceINRight", 100f)));

            Assert.That(Template.BounceOutFactory.Get(), Is.EqualTo(Template.Get("BounceOUT")));
            Assert.That(Template.BounceOutUpFactory.Get(100f), Is.EqualTo(Template.Get("BounceOUTup", 100f)));
            Assert.That(Template.BounceOutDownFactory.Get(100f), Is.EqualTo(Template.Get("BounceOUTdowN", 100f)));
            Assert.That(Template.BounceOutLeftFactory.Get(100f), Is.EqualTo(Template.Get("BounceOUTLeft", 100f)));
            Assert.That(Template.BounceOutRightFactory.Get(100f), Is.EqualTo(Template.Get("BounceOUTRight", 100f)));

            Assert.That(Template.FadeIn, Is.EqualTo(Template.Get("fadeIn")));
            Assert.That(Template.FadeInUp, Is.EqualTo(Template.Get("fadeInUp")));
            Assert.That(Template.FadeInDown, Is.EqualTo(Template.Get("fadeINdowN")));
            Assert.That(Template.FadeInLeft, Is.EqualTo(Template.Get("fadeinLeft")));
            Assert.That(Template.FadeInRight, Is.EqualTo(Template.Get("fadeinRight")));
            Assert.That(Template.FadeInTopLeft, Is.EqualTo(Template.Get("fadeinTOPLeft")));
            Assert.That(Template.FadeInTopRight, Is.EqualTo(Template.Get("fadeinTOPRight")));
            Assert.That(Template.FadeInBottomLeft, Is.EqualTo(Template.Get("fadeinBOTTOMLeft")));
            Assert.That(Template.FadeInBottomRight, Is.EqualTo(Template.Get("fadeinBOTTOMRight")));
            Assert.That(Template.FadeInUpBig, Is.EqualTo(Template.Get("fadeInUpBIG")));
            Assert.That(Template.FadeInDownBig, Is.EqualTo(Template.Get("fadeINdowNBIG")));
            Assert.That(Template.FadeInLeftBig, Is.EqualTo(Template.Get("fadeinLeftBIG")));
            Assert.That(Template.FadeInRightBig, Is.EqualTo(Template.Get("fadeinRightBIG")));

            Assert.That(Template.FadeOut, Is.EqualTo(Template.Get("fadeOut")));
            Assert.That(Template.FadeOutUp, Is.EqualTo(Template.Get("fadeOutUp")));
            Assert.That(Template.FadeOutDown, Is.EqualTo(Template.Get("fadeOutdowN")));
            Assert.That(Template.FadeOutLeft, Is.EqualTo(Template.Get("fadeOutLeft")));
            Assert.That(Template.FadeOutRight, Is.EqualTo(Template.Get("fadeOutRight")));
            Assert.That(Template.FadeOutTopLeft, Is.EqualTo(Template.Get("fadeOutTOPLeft")));
            Assert.That(Template.FadeOutTopRight, Is.EqualTo(Template.Get("fadeOutTOPRight")));
            Assert.That(Template.FadeOutBottomLeft, Is.EqualTo(Template.Get("fadeOutBOTTOMLeft")));
            Assert.That(Template.FadeOutBottomRight, Is.EqualTo(Template.Get("fadeOutBOTTOMRight")));
            Assert.That(Template.FadeOutUpBig, Is.EqualTo(Template.Get("fadeOutUpBIG")));
            Assert.That(Template.FadeOutDownBig, Is.EqualTo(Template.Get("fadeOutdowNBIG")));
            Assert.That(Template.FadeOutLeftBig, Is.EqualTo(Template.Get("fadeOutLeftBIG")));
            Assert.That(Template.FadeOutRightBig, Is.EqualTo(Template.Get("fadeOutRightBIG")));

            Assert.That(Template.LightSpeedInLeft, Is.EqualTo(Template.Get("lightSPEEDinLeft")));
            Assert.That(Template.LightSpeedInRight, Is.EqualTo(Template.Get("lightSPEEDinRight")));
            Assert.That(Template.LightSpeedOutLeft, Is.EqualTo(Template.Get("lightSPEEDoutLeft")));
            Assert.That(Template.LightSpeedOutRight, Is.EqualTo(Template.Get("lightSPEEDoutRight")));

            Assert.That(Template.RotateIn, Is.EqualTo(Template.Get("ROTATEIN")));
            Assert.That(Template.RotateInDownLeft, Is.EqualTo(Template.Get("ROTATEInDownLeft")));
            Assert.That(Template.RotateInDownRight, Is.EqualTo(Template.Get("ROTATEInDownRight")));
            Assert.That(Template.RotateInUpLeft, Is.EqualTo(Template.Get("rOTATEInUpLeft")));
            Assert.That(Template.RotateInUpRight, Is.EqualTo(Template.Get("ROTATEInUpRight")));

            Assert.That(Template.Hinge, Is.EqualTo(Template.Get("hInge")));
            Assert.That(Template.JackInTheBox, Is.EqualTo(Template.Get("jAckInTheBox")));
            Assert.That(Template.RollOutLeft, Is.EqualTo(Template.Get("rOllOutLeft")));
            Assert.That(Template.RollOutRight, Is.EqualTo(Template.Get("rOllOutRight")));
            Assert.That(Template.RollInLeft, Is.EqualTo(Template.Get("rOllInLEFT")));
            Assert.That(Template.RollInRight, Is.EqualTo(Template.Get("rOllInright")));
        }
    }
}