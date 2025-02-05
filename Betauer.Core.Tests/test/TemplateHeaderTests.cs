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
        TemplateHeader.RegisterAlias("CROSS", DirectionFlags.Up | DirectionFlags.Down |
                                        DirectionFlags.Right | DirectionFlags.Left);
        TemplateHeader.RegisterAlias("ALL", DirectionFlags.Up | DirectionFlags.Down |
                                      DirectionFlags.Right | DirectionFlags.Left);
        TemplateHeader.RegisterAlias("UD", DirectionFlags.Up | DirectionFlags.Down);
        TemplateHeader.RegisterAlias("TLBR", DirectionFlags.UpLeft | DirectionFlags.DownRight);
    }

    [Test]
    public void TestTemplateIdTypeToDirectionsString() {
        // Basic cardinal directions
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.Up), Is.EqualTo("U"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.Right), Is.EqualTo("R"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.Down), Is.EqualTo("D"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.Left), Is.EqualTo("L"));

        // Basic diagonal directions
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.UpRight), Is.EqualTo("UR"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.DownRight), Is.EqualTo("DR"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.DownLeft), Is.EqualTo("DL"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)DirectionFlags.UpLeft), Is.EqualTo("UL"));

        // Combined cardinal directions
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.Up | DirectionFlags.Down)), Is.EqualTo("D-U"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.Right | DirectionFlags.Left)), Is.EqualTo("L-R"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.Up | DirectionFlags.Right)), Is.EqualTo("R-U"));

        // Combined diagonal directions
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.UpRight | DirectionFlags.DownLeft)), Is.EqualTo("DL-UR"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.DownRight | DirectionFlags.UpLeft)), Is.EqualTo("DR-UL"));

        // Mix of cardinal and diagonal
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.Up | DirectionFlags.DownRight)), Is.EqualTo("DR-U"));
        Assert.That(TemplateHeader.TypeToDirectionsString((int)(DirectionFlags.Left | DirectionFlags.UpRight)), Is.EqualTo("L-UR"));
    }

    [Test]
    public void TestParseDirections() {
        // Basic cardinal directions
        Assert.That(TemplateHeader.Parse("N").Type, Is.EqualTo((int)DirectionFlags.Up));
        Assert.That(TemplateHeader.Parse("E").Type, Is.EqualTo((int)DirectionFlags.Right));
        Assert.That(TemplateHeader.Parse("S").Type, Is.EqualTo((int)DirectionFlags.Down));
        Assert.That(TemplateHeader.Parse("W").Type, Is.EqualTo((int)DirectionFlags.Left));

        // Basic diagonal directions
        Assert.That(TemplateHeader.Parse("NE").Type, Is.EqualTo((int)DirectionFlags.UpRight));
        Assert.That(TemplateHeader.Parse("SE").Type, Is.EqualTo((int)DirectionFlags.DownRight));
        Assert.That(TemplateHeader.Parse("SW").Type, Is.EqualTo((int)DirectionFlags.DownLeft));
        Assert.That(TemplateHeader.Parse("NW").Type, Is.EqualTo((int)DirectionFlags.UpLeft));

        // Combined cardinal directions
        Assert.That(TemplateHeader.Parse("N-S").Type, Is.EqualTo((int)(DirectionFlags.Up | DirectionFlags.Down)));
        Assert.That(TemplateHeader.Parse("E-W").Type, Is.EqualTo((int)(DirectionFlags.Right | DirectionFlags.Left)));
        Assert.That(TemplateHeader.Parse("N-E").Type, Is.EqualTo((int)(DirectionFlags.Up | DirectionFlags.Right)));
    }

    [Test]
    public void TestDirectionAliases() {
        // Test cardinal direction aliases
        Assert.That(TemplateHeader.Parse("U").Type, Is.EqualTo((int)DirectionFlags.Up));
        Assert.That(TemplateHeader.Parse("T").Type, Is.EqualTo((int)DirectionFlags.Up));
        Assert.That(TemplateHeader.Parse("R").Type, Is.EqualTo((int)DirectionFlags.Right));
        Assert.That(TemplateHeader.Parse("D").Type, Is.EqualTo((int)DirectionFlags.Down));
        Assert.That(TemplateHeader.Parse("B").Type, Is.EqualTo((int)DirectionFlags.Down));
        Assert.That(TemplateHeader.Parse("L").Type, Is.EqualTo((int)DirectionFlags.Left));

        // Test diagonal aliases
        Assert.That(TemplateHeader.Parse("TR").Type, Is.EqualTo((int)DirectionFlags.UpRight));
        Assert.That(TemplateHeader.Parse("BR").Type, Is.EqualTo((int)DirectionFlags.DownRight));
        Assert.That(TemplateHeader.Parse("BL").Type, Is.EqualTo((int)DirectionFlags.DownLeft));
        Assert.That(TemplateHeader.Parse("TL").Type, Is.EqualTo((int)DirectionFlags.UpLeft));

        Assert.That(TemplateHeader.Parse("UR").Type, Is.EqualTo((int)DirectionFlags.UpRight));
        Assert.That(TemplateHeader.Parse("DR").Type, Is.EqualTo((int)DirectionFlags.DownRight));
        Assert.That(TemplateHeader.Parse("DL").Type, Is.EqualTo((int)DirectionFlags.DownLeft));
        Assert.That(TemplateHeader.Parse("UL").Type, Is.EqualTo((int)DirectionFlags.UpLeft));
    }

    [Test]
    public void TestParseWithFlags() {
        // Test parsing with flags
        var template = TemplateHeader.Parse("N-SE/flag1/flag2");
        Assert.That(template.Type, Is.EqualTo((int)(DirectionFlags.Up | DirectionFlags.DownRight)));
        Assert.That(template.Flags, Is.EquivalentTo(new[] { "flag1", "flag2" }));

        template = TemplateHeader.Parse("NE-SW/special");
        Assert.That(template.Type, Is.EqualTo((int)(DirectionFlags.UpRight | DirectionFlags.DownLeft)));
        Assert.That(template.Flags, Is.EquivalentTo(new[] { "special" }));
    }

    [Test]
    public void TestNumericParsing() {
        // Test parsing numeric values
        Assert.That(TemplateHeader.Parse("1").Type, Is.EqualTo((int)DirectionFlags.Up));            // 1 = North (1)
        Assert.That(TemplateHeader.Parse("3").Type, Is.EqualTo((int)(DirectionFlags.Up |
                                                                 DirectionFlags.UpRight)));        // 3 = North (1) + NorthEast (2)
        Assert.That(TemplateHeader.Parse("15").Type, Is.EqualTo((int)(DirectionFlags.Up |          // 15 = North (1) +
                                                                  DirectionFlags.UpRight |        //      NorthEast (2) +
                                                                  DirectionFlags.Right |             //      East (4) +
                                                                  DirectionFlags.DownRight)));      //      SouthEast (8)
    }
    [Test]
    public void TestInvalidInput() {
        // Test invalid inputs
        Assert.Throws<ArgumentException>(() => TemplateHeader.Parse("X"));
        Assert.Throws<ArgumentException>(() => TemplateHeader.Parse("NSX"));
        Assert.Throws<ArgumentException>(() => TemplateHeader.Parse("N-X"));
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
            var template = TemplateHeader.Parse(testCase);
            var roundTrip = TemplateHeader.TypeToDirectionsString(template.Type);
            Assert.That(roundTrip, Is.EqualTo(testCase),
                $"Round trip failed for {testCase}. Got {roundTrip}");
        }
    }

    [Test]
    public void TestRegisterCustomAlias() {
        // Register a custom alias
        TemplateHeader.RegisterAlias("CROSS", DirectionFlags.Up | DirectionFlags.Down |
                                        DirectionFlags.Right | DirectionFlags.Left);

        // Test the custom alias
        Assert.That(TemplateHeader.Parse("CROSS").Type,
            Is.EqualTo((int)(DirectionFlags.Up | DirectionFlags.Down |
                            DirectionFlags.Right | DirectionFlags.Left)));

        // Test combining custom alias with regular directions
        Assert.That(TemplateHeader.Parse("CROSS-NE").Type,
            Is.EqualTo((int)(DirectionFlags.Up | DirectionFlags.Down |
                            DirectionFlags.Right | DirectionFlags.Left | DirectionFlags.UpRight)));
    }

    [Test]
    public void TestConsistentStringOutput() {
        // Registrar alias necesarios para el test
        TemplateHeader.RegisterAlias("TLBR", DirectionFlags.UpLeft | DirectionFlags.DownRight);
        TemplateHeader.RegisterAlias("UD", DirectionFlags.Up | DirectionFlags.Down);
        TemplateHeader.RegisterAlias("ALL", DirectionFlags.Up | DirectionFlags.Down |
                                      DirectionFlags.Right | DirectionFlags.Left);

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
            var template = TemplateHeader.Parse(input);
            var output = TemplateHeader.TypeToDirectionsString(template.Type);
            Assert.That(output, Is.EqualTo(expected),
                $"Input '{input}' should produce '{expected}' but got '{output}'");
        }
    }
}