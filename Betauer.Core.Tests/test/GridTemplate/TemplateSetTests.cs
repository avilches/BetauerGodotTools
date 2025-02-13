using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Betauer.Core.DataMath;
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
            @ dir=17
            abc
            def
            ghi

            @ DIR="U-D"
            xyz
            uvw
            rst

            @ Dir="T-s"
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
    public void LoadFromString_Filter() {
        var content = """
            @ dir=17
            abc
            def
            ghi

            @ DIR=17
            xyz
            uvw
            rst

            @ Dir=17 nope
            xyz
            uvw
            rst
        """;

        _templateSet.LoadFromString(content, template => !template.Tags.Contains("nope"));
        Assert.That(_templateSet.FindTemplates(17).ToArray(), Has.Length.EqualTo(2));
    }

    [Test]
    public void LoadFromString_Id() {
        var content = """
            @ ID=23 dir=17
            abc
            def
            ghi
        """;

        _templateSet.LoadFromString(content);
        Assert.That(_templateSet.GetTemplateById("23"), Is.Not.Null);
        Assert.That(_templateSet.GetTemplateById("NO"), Is.Null);
    }

    [Test]
    public void LoadFromString_Id_NoDuplicates() {
        var content = """
            @ ID=23 dir=17
            abc
            def
            ghi
            @ ID=23 dir=17
            abc
            def
            ghi
        """;

        Assert.Throws<ArgumentException>(() => _templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_AutoId() {
        var content = """
            @ dir=17
            abc
            def
            ghi
        """;

        _templateSet.LoadFromString(content);
        var template = _templateSet.FindTemplates().ToArray()[0];
        Assert.That(template.Id, Is.EqualTo(RuntimeHelpers.GetHashCode(template).ToString()));
    }

    [Test]
    public void LoadFromString_Tags() {
        var content = """
            @ dir=15 tag1 tag2 tag3,tag4,Tag4
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
            @ dir=15 tag1 tag2 tag3,tag4 pepe=1 juan="2" dir:N:name:l=dos dir:N:name=tres
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
        Assert.That(template.GetAttribute("dir:U:name:l"), Is.EqualTo("dos"));
        Assert.That(template.GetAttribute("dir:U:name"), Is.EqualTo("tres"));
        Assert.That(template.GetAttribute(DirectionFlag.Up, "name:l"), Is.EqualTo("dos"));
        Assert.That(template.GetAttribute(DirectionFlag.Up, "name"), Is.EqualTo("tres"));

        var rotated = template.Transform(Transformations.Type.Rotate90);

        Assert.That(rotated.GetAttribute("dir:R:name:l"), Is.EqualTo("dos"));
        Assert.That(rotated.GetAttribute("dir:R:name"), Is.EqualTo("tres"));
        Assert.That(rotated.GetAttribute(DirectionFlag.Right, "name:l"), Is.EqualTo("dos"));
        Assert.That(rotated.GetAttribute(DirectionFlag.Right, "name"), Is.EqualTo("tres"));

    }

    [Test]
    public void LoadFromString_DefineTemplate() {
        var content = """
                          @ DEFINE=corridor flag1,flag2 dir=T-R fromDefine=hola
                          
                          @ TEMPLATE=corridor
                          abc
                          def
                          ghi
                      """;

        _templateSet.LoadFromString(content);
        var template = _templateSet.GetTemplate((byte)DirectionFlag.Up | (byte)DirectionFlag.Right)!;

        Assert.That(template.Tags, Is.EquivalentTo(new[] { "flag1", "flag2" }));
        Assert.That(template.GetAttribute("fromDefine"), Is.EqualTo("hola"));
    }

    [Test]
    public void LoadFromString_DefineTemplate_AvoidDuplicate() {
        var content = """
                          @ Define=corridor1
                          @ Define=corridor2
                          
                          @ Template=corridor1
                          @ Template=corridor2
                          abc
                          def
                          ghi
                      """;

        Assert.Throws<ArgumentException>(() =>_templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_DefineTemplate_NotFound() {
        var content = """
                          @ Template=corridor1
                          abc
                          def
                          ghi
                      """;

        Assert.Throws<ArgumentException>(() =>_templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_TooManyLines() {
        var content = """
                          @ dir=N
                          abc
                          def
                          ghi
                          ghi
                      """;

        Assert.Throws<ArgumentException>(() =>_templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_TooFewLines() {
        var content = """
                          @ dir=N
                          abc
                          def
                      """;

        Assert.Throws<ArgumentException>(() =>_templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_WrongWidth() {
        var content = """
                          @ dir=N
                          abc
                          def
                          ghij
                      """;

        Assert.Throws<ArgumentException>(() =>_templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_NoDir() {
        var content = """
                      @ d=a
                      abc
                      def
                      ghi
                      """;

        Assert.Throws<ArgumentException>(() =>_templateSet.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_DefineTemplate_Overwrites() {
        var content = """
                          @ Define=corridor flag1,flag2 dir=S fromDefine=hola
                          @ Define=corridor2 fromDefine=otro 
                          @ Define=corridor fromDefine=overwritten fromDefine2=hola fromDefine3=hola3
                          
                          @ mores=mores flag5
                          @ Template=corridor dir=N flag3,flag4 fromTemplate=adios fromDefine3=overwritten2
                          @ morex=morex flag6
                          abc
                          def
                          ghi
                      """;

        _templateSet.LoadFromString(content);
        var template = _templateSet.GetTemplate((byte)DirectionFlag.Up)!;

        Assert.That(template.Tags, Is.EquivalentTo(new[] { "flag1", "flag2", "flag3", "flag4",  "flag5", "flag6" }));
        Assert.That(template.GetAttribute("fromDefine"), Is.EqualTo("overwritten"));
        Assert.That(template.GetAttribute("fromDefine2"), Is.EqualTo("hola"));
        Assert.That(template.GetAttribute("fromTemplate"), Is.EqualTo("adios"));
        Assert.That(template.GetAttribute("fromDefine3"), Is.EqualTo("overwritten2"));
        Assert.That(template.GetAttribute("mores"), Is.EqualTo("mores"));
        Assert.That(template.GetAttribute("morex"), Is.EqualTo("morex"));
    }

    [Test]
    public void FindTemplates_WithRequiredFlags_ReturnsMatchingTemplates() {
        var content = """
            @ dir=15
            abc
            def
            ghi
            @ dir=15 special,extra
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
            @ dir=15 flag1
            abc
            def
            ghi
            @ dir=15 flag1,flag2
            xyz
            uvw
            rst
            @ dir=15 flag1,flag2,flag3
            123
            456
            789
        """;

        _templateSet.LoadFromString(content);
        var templates = _templateSet.FindTemplates(15, new[] { "flag1" }).ToArray();

        Assert.That(templates, Has.Length.EqualTo(3));
    }

    [Test]
    public void FindTemplates_WithRequiredAndOptionalFlags_ReturnsOrderedTemplates_WithDefine() {
        var content = """
            @ Define=corridor flag1 dir=15
            
            @ Template=corridor
            abc
            def
            ghi
            @ Template=corridor dir=15 flag2
            xyz
            uvw
            rst
            @ Template=corridor dir=15 flag2,flag3
            123
            456
            789
        """;

        _templateSet.LoadFromString(content);
        Assert.That(_templateSet.FindTemplates(15, ["flag1"]).ToArray(), Has.Length.EqualTo(3));
        Assert.That(_templateSet.FindTemplates(15, ["flag1", "flag2"]).ToArray(), Has.Length.EqualTo(2));
        Assert.That(_templateSet.FindTemplates(15, ["flag1", "flag2", "flag3"]).ToArray(), Has.Length.EqualTo(1));
    }

    [Test]
    public void GetTemplate_WithExactFlags_ReturnsUniqueTemplate() {
        var content = """
            @ dir=15 flag1,flag2
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
            @ dir=15
            abc
            def
            ghi
        """;

        _templateSet.LoadFromString(content);
        Assert.That(_templateSet.FindTemplates(15, new[] { "nonexistent" }).ToArray(), Is.Empty);
    }
}