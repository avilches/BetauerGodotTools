using System;
using NUnit.Framework;
using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using Godot;

namespace Betauer.Core.Tests.GridTemplate;

[TestFixture]
public class TemplateValidationTests {
    private static readonly Func<char, bool> IsBlocked = (c) => c != '·' && c != 'd';

    [Test]
    public void SingleExit() {
        var template = new Template {
            Body = Array2D.Parse(@"
                #####
                #####
                #####
                #####
                ##·##"),
            DirectionFlags = (byte)DirectionFlag.Down
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.True);
    }

    [Test]
    public void SingleExit_NotValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                #####
                #####
                #####
                #####
                #####"),
            DirectionFlags = (byte)DirectionFlag.Down
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.False);
    }

    [TestCase(3, true)]
    [TestCase(1, false)]
    public void SingleExit_IsValid_Size3(int size, bool expected) {
        var template = new Template {
            Body = Array2D.Parse(@"
                #####
                #####
                #####
                #####
                #···#"),
            DirectionFlags = (byte)DirectionFlag.Down
        };
        template.SetAttribute(DirectionFlag.Down, "size", size);

        Assert.That(template.IsValid(IsBlocked), Is.EqualTo(expected));
    }

    [Test]
    public void TwoExits_Connected_IsValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                ##·##
                #··##
                #····
                #··##
                #####"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right)
        };
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        template.SetAttribute(DirectionFlag.Right, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.True);
    }

    [Test]
    public void MoreThanTwoExits_Connected_IsNotValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                ##·##
                #··##
                ·····
                #··##
                #####"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right)
        };
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        template.SetAttribute(DirectionFlag.Right, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.False);
    }

    [Test]
    public void TwoExits_Blocked_IsNotValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                #·###
                #·###
                #####
                #··##
                #·###"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Down)
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.False);
    }

    [Test]
    public void ThreeExits_Connected_IsValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                ##·##
                #··##
                #····
                #··##
                ##·##"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.Down)
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        template.SetAttribute(DirectionFlag.Right, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.True);
    }

    [Test]
    public void ThreeExits_OneBlocked_IsNotValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                #·###
                #·###
                ####·
                #··##
                #·###"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.Down)
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        template.SetAttribute(DirectionFlag.Right, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.False);
    }

    [Test]
    public void NoExits_IsValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                #####
                #···#
                #···#
                #···#
                #####"),
            DirectionFlags = 0
        };
        Assert.That(template.IsValid(IsBlocked), Is.True);
    }

    [Test]
    public void FourExits_Connected_IsValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                ##·##
                #··##
                ·····
                #··##
                ##·##"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.Down | DirectionFlag.Left)
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        template.SetAttribute(DirectionFlag.Left, "size", 1);
        template.SetAttribute(DirectionFlag.Right, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.True);
    }

    [Test]
    public void WithDoors_IsValid() {
        var template = new Template {
            Body = Array2D.Parse(@"
                ##d##
                #··##
                d···d
                #··##
                ##d##"),
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.Down | DirectionFlag.Left)
        };
        template.SetAttribute(DirectionFlag.Down, "size", 1);
        template.SetAttribute(DirectionFlag.Up, "size", 1);
        template.SetAttribute(DirectionFlag.Left, "size", 1);
        template.SetAttribute(DirectionFlag.Right, "size", 1);
        Assert.That(template.IsValid(IsBlocked), Is.True);
    }
}