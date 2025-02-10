using System.Linq;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.GridTemplate;

[TestFixture]
public class TemplateSetTests {
    private TemplateSet _templateSet;

    [SetUp]
    public void Setup() {
        _templateSet = new TemplateSet(3);
    }

    [Test]
    public void Constructor_SetsCorrectCellSize() {
        Assert.That(_templateSet.CellSize, Is.EqualTo(3));
    }

    [Test]
    public void LoadFromString_DirectionFlags() {
        var content = """
            @ Template=17
            abc
            def
            ghi

            @ Template="U-D"
            xyz
            uvw
            rst

            @ Template="T-s"
            xyz
            uvw
            rst
        """;

        _templateSet.LoadFromString(content);
        Assert.That(_templateSet.FindTemplates(17).ToArray(), Has.Length.EqualTo(3));
        Assert.That(_templateSet.FindTemplates(DirectionFlagTools.StringToFlags("U-D")).ToArray(), Has.Length.EqualTo(3));
        Assert.That(_templateSet.FindTemplates(DirectionFlagTools.StringToFlags("s-T")).ToArray(), Has.Length.EqualTo(3));
        Assert.That(_templateSet.FindTemplates((byte)(DirectionFlag.Up | DirectionFlag.Down)).ToArray(), Has.Length.EqualTo(3));
    }

    [Test]
    public void LoadFromString_Tags() {
        var content = """
            @ Template=15 tag1 tag2 tag3,tag4,Tag4
            abc
            def
            ghi
        """;
        
        _templateSet.LoadFromString(content);
        var template = _templateSet.FindTemplates(15).ToArray()[0];

        Assert.That(template.Tags, Is.EquivalentTo(new[] { "tag1", "tag2", "tag3", "tag4" }));

    }

    [Test]
    public void LoadFromString_Attributes() {
        var content = """
            @ Template=15 tag1 tag2 tag3,tag4 pepe=1 juan="2" dir:N=tres
            abc
            def
            ghi
        """;

        _templateSet.LoadFromString(content);
        var template = _templateSet.FindTemplates(15).ToArray()[0];

        Assert.That(template.Tags, Is.EquivalentTo(new[] { "tag1", "tag2", "tag3", "tag4" }));
        Assert.That(template.GetAttribute("pepe"), Is.EqualTo(1));
        Assert.That(template.GetAttribute("juan"), Is.EqualTo("2"));

        // N (north) is normalized to U (up)
        Assert.That(template.GetAttribute("dir:U"), Is.EqualTo("tres"));
        Assert.That(template.GetAttribute(DirectionFlag.Up), Is.EqualTo("tres"));
        Assert.That(template.GetAttribute("dir:"+DirectionFlagTools.NormalizeFlag("t")), Is.EqualTo("tres"));

    }

    [Test]
    public void FindTemplates_WithRequiredFlags_ReturnsMatchingTemplates() {
        var content = """
            @ Template=15
            abc
            def
            ghi
            @ Template=15 special,extra
            xyz
            uvw
            rst
        """;

        _templateSet.LoadFromString(content);
        var templates = _templateSet.FindTemplates(15, new[] { "special" }).ToArray();

        Assert.That(templates, Has.Length.EqualTo(1));
    }

    [Test]
    public void FindTemplates_WithRequiredAndOptionalFlags_ReturnsOrderedTemplates() {
        var content = """
            @ Template=15 flag1
            abc
            def
            ghi
            @ Template=15 flag1,flag2
            xyz
            uvw
            rst
            @ Template=15 flag1,flag2,flag3
            123
            456
            789
        """;

        _templateSet.LoadFromString(content);
        var templates = _templateSet.FindTemplates(15, new[] { "flag1" }).ToArray();

        Assert.That(templates, Has.Length.EqualTo(3));
    }

    [Test]
    public void GetTemplate_WithExactFlags_ReturnsUniqueTemplate() {
        var content = """
            @ Template=15 flag1,flag2
            abc
            def
            ghi
        """;

        _templateSet.LoadFromString(content);
        var template = _templateSet.GetTemplate(15, new[] { "flag1", "flag2" });

        Assert.That(template, Is.Not.Null);
    }

    [Test]
    public void FindTemplates_WithNonExistentType_ThrowsException() {
        Assert.That(_templateSet.FindTemplates(99).ToArray(), Is.Empty);
    }

    [Test]
    public void FindTemplates_WithNonExistentFlags_ThrowsException() {
        var content = """
            @ Template=15
            abc
            def
            ghi
        """;

        _templateSet.LoadFromString(content);
        Assert.That(_templateSet.FindTemplates(15, new[] { "nonexistent" }).ToArray(), Is.Empty);
    }
}