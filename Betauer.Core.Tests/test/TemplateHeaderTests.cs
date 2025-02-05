using System;
using System.Collections.Generic;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class TemplateHeaderTests {
    [SetUp]
    public void Setup() {
        // Registrar alias para los tests
        DirectionFlagTools.RegisterAlias("CROSS", DirectionFlag.Up | DirectionFlag.Down |
                                        DirectionFlag.Right | DirectionFlag.Left);
        DirectionFlagTools.RegisterAlias("ALL", DirectionFlag.Up | DirectionFlag.Down |
                                                DirectionFlag.Right | DirectionFlag.Left);
        DirectionFlagTools.RegisterAlias("UD", DirectionFlag.Up | DirectionFlag.Down);
        DirectionFlagTools.RegisterAlias("TLBR", DirectionFlag.UpLeft | DirectionFlag.DownRight);
    }

    [Test]
    public void TestTemplateIdTypeToDirectionsString() {
        // Basic cardinal directions
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.Up), Is.EqualTo("U"));
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.Right), Is.EqualTo("R"));
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.Down), Is.EqualTo("D"));
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.Left), Is.EqualTo("L"));

        // Basic diagonal directions
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.UpRight), Is.EqualTo("UR"));
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.DownRight), Is.EqualTo("DR"));
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.DownLeft), Is.EqualTo("DL"));
        Assert.That(DirectionFlagTools.FlagsToString((int)DirectionFlag.UpLeft), Is.EqualTo("UL"));

        // Combined cardinal directions
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.Up | DirectionFlag.Down)), Is.EqualTo("D-U"));
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.Right | DirectionFlag.Left)), Is.EqualTo("L-R"));
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.Up | DirectionFlag.Right)), Is.EqualTo("R-U"));

        // Combined diagonal directions
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.UpRight | DirectionFlag.DownLeft)), Is.EqualTo("DL-UR"));
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.DownRight | DirectionFlag.UpLeft)), Is.EqualTo("DR-UL"));

        // Mix of cardinal and diagonal
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.Up | DirectionFlag.DownRight)), Is.EqualTo("DR-U"));
        Assert.That(DirectionFlagTools.FlagsToString((int)(DirectionFlag.Left | DirectionFlag.UpRight)), Is.EqualTo("L-UR"));
    }

    [Test]
    public void TestParseDirections() {
        // Basic cardinal directions
        Assert.That(DirectionFlagTools.StringToFlags("N"), Is.EqualTo((int)DirectionFlag.Up));
        Assert.That(DirectionFlagTools.StringToFlags("E"), Is.EqualTo((int)DirectionFlag.Right));
        Assert.That(DirectionFlagTools.StringToFlags("S"), Is.EqualTo((int)DirectionFlag.Down));
        Assert.That(DirectionFlagTools.StringToFlags("W"), Is.EqualTo((int)DirectionFlag.Left));

        // Basic diagonal directions
        Assert.That(DirectionFlagTools.StringToFlags("NE"), Is.EqualTo((int)DirectionFlag.UpRight));
        Assert.That(DirectionFlagTools.StringToFlags("SE"), Is.EqualTo((int)DirectionFlag.DownRight));
        Assert.That(DirectionFlagTools.StringToFlags("SW"), Is.EqualTo((int)DirectionFlag.DownLeft));
        Assert.That(DirectionFlagTools.StringToFlags("NW"), Is.EqualTo((int)DirectionFlag.UpLeft));

        // Combined cardinal directions
        Assert.That(DirectionFlagTools.StringToFlags("N-S"), Is.EqualTo((int)(DirectionFlag.Up | DirectionFlag.Down)));
        Assert.That(DirectionFlagTools.StringToFlags("E-W"), Is.EqualTo((int)(DirectionFlag.Right | DirectionFlag.Left)));
        Assert.That(DirectionFlagTools.StringToFlags("N-E"), Is.EqualTo((int)(DirectionFlag.Up | DirectionFlag.Right)));
    }

    [Test]
    public void TestDirectionAliases() {
        // Test cardinal direction aliases
        Assert.That(DirectionFlagTools.StringToFlags("U"), Is.EqualTo((int)DirectionFlag.Up));
        Assert.That(DirectionFlagTools.StringToFlags("T"), Is.EqualTo((int)DirectionFlag.Up));
        Assert.That(DirectionFlagTools.StringToFlags("R"), Is.EqualTo((int)DirectionFlag.Right));
        Assert.That(DirectionFlagTools.StringToFlags("D"), Is.EqualTo((int)DirectionFlag.Down));
        Assert.That(DirectionFlagTools.StringToFlags("B"), Is.EqualTo((int)DirectionFlag.Down));
        Assert.That(DirectionFlagTools.StringToFlags("L"), Is.EqualTo((int)DirectionFlag.Left));

        // Test diagonal aliases
        Assert.That(DirectionFlagTools.StringToFlags("TR"), Is.EqualTo((int)DirectionFlag.UpRight));
        Assert.That(DirectionFlagTools.StringToFlags("BR"), Is.EqualTo((int)DirectionFlag.DownRight));
        Assert.That(DirectionFlagTools.StringToFlags("BL"), Is.EqualTo((int)DirectionFlag.DownLeft));
        Assert.That(DirectionFlagTools.StringToFlags("TL"), Is.EqualTo((int)DirectionFlag.UpLeft));

        Assert.That(DirectionFlagTools.StringToFlags("UR"), Is.EqualTo((int)DirectionFlag.UpRight));
        Assert.That(DirectionFlagTools.StringToFlags("DR"), Is.EqualTo((int)DirectionFlag.DownRight));
        Assert.That(DirectionFlagTools.StringToFlags("DL"), Is.EqualTo((int)DirectionFlag.DownLeft));
        Assert.That(DirectionFlagTools.StringToFlags("UL"), Is.EqualTo((int)DirectionFlag.UpLeft));
    }

    [Test]
    public void TestNumericParsing() {
        // Test parsing numeric values
        Assert.That(DirectionFlagTools.StringToFlags("1"), Is.EqualTo((int)DirectionFlag.Up));            // 1 = North (1)
        Assert.That(DirectionFlagTools.StringToFlags("3"), Is.EqualTo((int)(DirectionFlag.Up |
                                                                 DirectionFlag.UpRight)));        // 3 = North (1) + NorthEast (2)
        Assert.That(DirectionFlagTools.StringToFlags("15"), Is.EqualTo((int)(DirectionFlag.Up |          // 15 = North (1) +
                                                                  DirectionFlag.UpRight |        //      NorthEast (2) +
                                                                  DirectionFlag.Right |             //      East (4) +
                                                                  DirectionFlag.DownRight)));      //      SouthEast (8)
    }
    [Test]
    public void TestInvalidInput() {
        // Test invalid inputs
        Assert.Throws<ArgumentException>(() => DirectionFlagTools.StringToFlags("X"));
        Assert.Throws<ArgumentException>(() => DirectionFlagTools.StringToFlags("NSX"));
        Assert.Throws<ArgumentException>(() => DirectionFlagTools.StringToFlags("N-X"));
    }

    [Test]
    public void TestRoundTrip() {
        // Test that parsing and converting back to string produces consistent results
        var cases = new[] {
            "R-U",
            "L-R",
            "DL-UR",
            "DR-L-U",
            "D-L-R-U",
            "DL-DR-UL-UR"
        };

        foreach (var testCase in cases) {
            var template = DirectionFlagTools.StringToFlags(testCase);
            var roundTrip = DirectionFlagTools.FlagsToString(template);
            Assert.That(roundTrip, Is.EqualTo(testCase),
                $"Round trip failed for {testCase}. Got {roundTrip}");
        }
    }

    [Test]
    public void TestRegisterCustomAlias() {
        // Register a custom alias
        DirectionFlagTools.RegisterAlias("CROSS", DirectionFlag.Up | DirectionFlag.Down |
                                        DirectionFlag.Right | DirectionFlag.Left);

        // Test the custom alias
        Assert.That(DirectionFlagTools.StringToFlags("CROSS"),
            Is.EqualTo((int)(DirectionFlag.Up | DirectionFlag.Down |
                            DirectionFlag.Right | DirectionFlag.Left)));

        // Test combining custom alias with regular directions
        Assert.That(DirectionFlagTools.StringToFlags("CROSS-NE"),
            Is.EqualTo((int)(DirectionFlag.Up | DirectionFlag.Down |
                            DirectionFlag.Right | DirectionFlag.Left | DirectionFlag.UpRight)));
    }

    [Test]
    public void TestConsistentStringOutput() {
        // Registrar alias necesarios para el test
        DirectionFlagTools.RegisterAlias("TLBR", DirectionFlag.UpLeft | DirectionFlag.DownRight);
        DirectionFlagTools.RegisterAlias("UD", DirectionFlag.Up | DirectionFlag.Down);
        DirectionFlagTools.RegisterAlias("ALL", DirectionFlag.Up | DirectionFlag.Down |
                                      DirectionFlag.Right | DirectionFlag.Left);

        // Test that different input combinations produce consistent output
        var cases = new Dictionary<string, string> {
            { "U-R", "R-U" }, // Should be ordered alphabetically
            { "E-N", "R-U" },
            { "R-U", "R-U" },
            { "TLBR", "DR-UL" }, // Alias should be expanded to ordered directions
            { "UD-E", "D-R-U" }, // Combination of alias and direction
            { "ALL", "D-L-R-U" } // ALL alias expands to ordered cardinal directions
        };

        foreach (var (input, expected) in cases) {
            var template = DirectionFlagTools.StringToFlags(input);
            var output = DirectionFlagTools.FlagsToString(template);
            Assert.That(output, Is.EqualTo(expected),
                $"Input '{input}' should produce '{expected}' but got '{output}'");
        }
    }
}