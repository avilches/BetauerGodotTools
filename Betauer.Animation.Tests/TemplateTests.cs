using Betauer.Animation.Tween;
using Godot;
using NUnit.Framework;
using Betauer.TestRunner;

namespace Betauer.Animation.Tests {
    [TestFixture]
    public class TemplateTests : NodeTest {
        [Test]
        public void TemplateNamesCheck() {
            Assert.That(Templates.Bounce, Is.EqualTo(Templates.Get<KeyframeAnimation>("bOunce")));
            Assert.That(Templates.Flash, Is.EqualTo(Templates.Get<KeyframeAnimation>("fLash")));
            Assert.That(Templates.HeadShake, Is.EqualTo(Templates.Get<KeyframeAnimation>("hEadshake")));
            Assert.That(Templates.HeartBeat, Is.EqualTo(Templates.Get<KeyframeAnimation>("hEartbeat")));
            Assert.That(Templates.Jello, Is.EqualTo(Templates.Get<KeyframeAnimation>("jEllo")));
            Assert.That(Templates.Pulse, Is.EqualTo(Templates.Get<KeyframeAnimation>("pUlse")));
            Assert.That(Templates.RubberBand, Is.EqualTo(Templates.Get<KeyframeAnimation>("rUbberband")));
            Assert.That(Templates.Shake, Is.EqualTo(Templates.Get<KeyframeAnimation>("sHake")));
            Assert.That(Templates.ShakeX, Is.EqualTo(Templates.Get<KeyframeAnimation>("sHakex")));
            Assert.That(Templates.ShakeY, Is.EqualTo(Templates.Get<KeyframeAnimation>("sHakey")));
            Assert.That(Templates.Swing, Is.EqualTo(Templates.Get<KeyframeAnimation>("sWing")));
            Assert.That(Templates.Tada, Is.EqualTo(Templates.Get<KeyframeAnimation>("tAda")));
            Assert.That(Templates.Wobble, Is.EqualTo(Templates.Get<KeyframeAnimation>("wObble")));
            Assert.That(Templates.BackInUp, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckInUp")));
            Assert.That(Templates.BackInDown, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckINdowN")));
            Assert.That(Templates.BackInLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckinLeft")));
            Assert.That(Templates.BackInRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckinRight")));
            Assert.That(Templates.BackOutUp, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckOUTup")));
            Assert.That(Templates.BackOutDown, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckOUTdowN")));
            Assert.That(Templates.BackOutLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckOUTLeft")));
            Assert.That(Templates.BackOutRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("bAckOUTRight")));

            Assert.That(Templates.BackInUpFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckInUp", 100f)));
            Assert.That(Templates.BackInDownFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckINdowN", 100f)));
            Assert.That(Templates.BackInLeftFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckinLeft", 100f)));
            Assert.That(Templates.BackInRightFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckinRight", 100f)));

            Assert.That(Templates.BackOutUpFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckOUTup", 100f)));
            Assert.That(Templates.BackOutDownFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckOUTdowN", 100f)));
            Assert.That(Templates.BackOutLeftFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckOUTLeft", 100f)));
            Assert.That(Templates.BackOutRightFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("bAckOUTRight", 100f)));

            Assert.That(Templates.BounceInFactory.Get(), Is.EqualTo(Templates.Get<KeyframeAnimation>("BounceIN")));
            Assert.That(Templates.BounceInUpFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceINup", 100f)));
            Assert.That(Templates.BounceInDownFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceINdowN", 100f)));
            Assert.That(Templates.BounceInLeftFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceINLeft", 100f)));
            Assert.That(Templates.BounceInRightFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceINRight", 100f)));

            Assert.That(Templates.BounceOutFactory.Get(), Is.EqualTo(Templates.Get<KeyframeAnimation>("BounceOUT")));
            Assert.That(Templates.BounceOutUpFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceOUTup", 100f)));
            Assert.That(Templates.BounceOutDownFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceOUTdowN", 100f)));
            Assert.That(Templates.BounceOutLeftFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceOUTLeft", 100f)));
            Assert.That(Templates.BounceOutRightFactory.Get(100f), Is.EqualTo(Templates.Get<float, KeyframeAnimation>("BounceOUTRight", 100f)));

            Assert.That(Templates.FadeIn, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeIn")));
            Assert.That(Templates.FadeInUp, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeInUp")));
            Assert.That(Templates.FadeInDown, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeINdowN")));
            Assert.That(Templates.FadeInLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinLeft")));
            Assert.That(Templates.FadeInRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinRight")));
            Assert.That(Templates.FadeInTopLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinTOPLeft")));
            Assert.That(Templates.FadeInTopRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinTOPRight")));
            Assert.That(Templates.FadeInBottomLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinBOTTOMLeft")));
            Assert.That(Templates.FadeInBottomRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinBOTTOMRight")));
            Assert.That(Templates.FadeInUpBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeInUpBIG")));
            Assert.That(Templates.FadeInDownBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeINdowNBIG")));
            Assert.That(Templates.FadeInLeftBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinLeftBIG")));
            Assert.That(Templates.FadeInRightBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeinRightBIG")));

            Assert.That(Templates.FadeOut, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOut")));
            Assert.That(Templates.FadeOutUp, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutUp")));
            Assert.That(Templates.FadeOutDown, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutdowN")));
            Assert.That(Templates.FadeOutLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutLeft")));
            Assert.That(Templates.FadeOutRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutRight")));
            Assert.That(Templates.FadeOutTopLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutTOPLeft")));
            Assert.That(Templates.FadeOutTopRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutTOPRight")));
            Assert.That(Templates.FadeOutBottomLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutBOTTOMLeft")));
            Assert.That(Templates.FadeOutBottomRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutBOTTOMRight")));
            Assert.That(Templates.FadeOutUpBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutUpBIG")));
            Assert.That(Templates.FadeOutDownBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutdowNBIG")));
            Assert.That(Templates.FadeOutLeftBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutLeftBIG")));
            Assert.That(Templates.FadeOutRightBig, Is.EqualTo(Templates.Get<KeyframeAnimation>("fadeOutRightBIG")));

            Assert.That(Templates.LightSpeedInLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("lightSPEEDinLeft")));
            Assert.That(Templates.LightSpeedInRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("lightSPEEDinRight")));
            Assert.That(Templates.LightSpeedOutLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("lightSPEEDoutLeft")));
            Assert.That(Templates.LightSpeedOutRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("lightSPEEDoutRight")));

            Assert.That(Templates.RotateIn, Is.EqualTo(Templates.Get<KeyframeAnimation>("ROTATEIN")));
            Assert.That(Templates.RotateInDownLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("ROTATEInDownLeft")));
            Assert.That(Templates.RotateInDownRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("ROTATEInDownRight")));
            Assert.That(Templates.RotateInUpLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("rOTATEInUpLeft")));
            Assert.That(Templates.RotateInUpRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("ROTATEInUpRight")));

            Assert.That(Templates.Hinge, Is.EqualTo(Templates.Get<KeyframeAnimation>("hInge")));
            Assert.That(Templates.JackInTheBox, Is.EqualTo(Templates.Get<KeyframeAnimation>("jAckInTheBox")));
            Assert.That(Templates.RollOutLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("rOllOutLeft")));
            Assert.That(Templates.RollOutRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("rOllOutRight")));
            Assert.That(Templates.RollInLeft, Is.EqualTo(Templates.Get<KeyframeAnimation>("rOllInLEFT")));
            Assert.That(Templates.RollInRight, Is.EqualTo(Templates.Get<KeyframeAnimation>("rOllInright")));
        }
    }
}