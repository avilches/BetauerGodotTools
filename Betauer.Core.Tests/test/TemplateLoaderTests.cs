using System;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
public class TemplateLoaderTests {
    [Test]
    public void Constructor_WithValidCellSize_CreatesInstance() {
        Assert.DoesNotThrow(() => new TemplateLoader(3));
    }

    [Test]
    public void Constructor_WithInvalidCellSize_ThrowsException() {
        Assert.Throws<ArgumentException>(() => new TemplateLoader(2));
        Assert.Throws<ArgumentException>(() => new TemplateLoader(1));
        Assert.Throws<ArgumentException>(() => new TemplateLoader(0));
        Assert.Throws<ArgumentException>(() => new TemplateLoader(-1));
    }

    [Test]
    public void LoadFromString_BasicTemplate_Success() {
        var loader = new TemplateLoader(3);
        var content = @"
            @ID=15
            abc
            def
            ghi
        ";
        var templates = loader.LoadFromString(content);
        
        Assert.That(templates, Contains.Key(15));
        Assert.That(templates[15], Has.Count.EqualTo(1));
        
        var template = templates[15][0];
        var data = template.Data;
        Assert.That(data[0, 0], Is.EqualTo('a'));
        Assert.That(data[0, 1], Is.EqualTo('b'));
        Assert.That(data[0, 2], Is.EqualTo('c'));
        Assert.That(data[1, 0], Is.EqualTo('d'));
        Assert.That(data[1, 1], Is.EqualTo('e'));
        Assert.That(data[1, 2], Is.EqualTo('f'));
        Assert.That(data[2, 0], Is.EqualTo('g'));
        Assert.That(data[2, 1], Is.EqualTo('h'));
        Assert.That(data[2, 2], Is.EqualTo('i'));
    }

    [Test]
    public void LoadFromString_TemplateWithFlags_Success() {
        var loader = new TemplateLoader(3);
        var content = @"
            @ID=17176/flag1/flag2
            abc
            def
            ghi
        ";
        var templates = loader.LoadFromString(content);
        
        Assert.That(templates, Contains.Key(17176));
        var template = templates[17176][0];
        Assert.That(template.Id.Flags, Is.EquivalentTo(new[] { "flag1", "flag2" }));
    }

    [Test]
    public void LoadFromString_DuplicateTemplate_ThrowsException() {
        var loader = new TemplateLoader(3);
        var content = @"
            @ID=15/flag1
            abc    
            def
            ghi
            @ID=15/flag1
            xyz
            uvw
            rst
        ";
        
        Assert.Throws<ArgumentException>(() => loader.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_InvalidDimensions_ThrowsException() {
        var loader = new TemplateLoader(3);
        var content = @"
            @ID=15
            ab
            def
            ghi
        ";
        
        Assert.Throws<ArgumentException>(() => loader.LoadFromString(content));
    }

    [Test]
    public void LoadFromString_TemplateInheritance_Success() {
        var loader = new TemplateLoader(3);
        var content = @"
            @ID=15
            abc
            def
            ghi
            @ID=15/special from 15
        ";
        var templates = loader.LoadFromString(content);
        
        Assert.That(templates[15], Has.Count.EqualTo(2));
        var baseTemplate = templates[15][0];
        var inheritedTemplate = templates[15][1];
        
        Assert.That(inheritedTemplate.Data.ToString(), Is.EqualTo(baseTemplate.Data.ToString()));
        Assert.That(inheritedTemplate.Id.Flags, Contains.Item("special"));
    }

    [Test]
    public void LoadFromString_TemplateWithTransformation_Success() {
        var loader = new TemplateLoader(3);
        var content = @"
            @ID=15
            abc
            def
            ghi
            @ID=15/rotated from 15:90
        ";
        var templates = loader.LoadFromString(content);
        
        var baseTemplate = templates[15][0];
        var transformedTemplate = templates[15][1];
        
        // 90 degree rotation should make:
        // abc    gda
        // def -> heb
        // ghi    ifc
        Assert.That(transformedTemplate.Data[0, 0], Is.EqualTo('g'));
        Assert.That(transformedTemplate.Data[0, 1], Is.EqualTo('d'));
        Assert.That(transformedTemplate.Data[0, 2], Is.EqualTo('a'));
        Assert.That(transformedTemplate.Data[1, 0], Is.EqualTo('h'));
        Assert.That(transformedTemplate.Data[1, 1], Is.EqualTo('e'));
        Assert.That(transformedTemplate.Data[1, 2], Is.EqualTo('b'));
        Assert.That(transformedTemplate.Data[2, 0], Is.EqualTo('i'));
        Assert.That(transformedTemplate.Data[2, 1], Is.EqualTo('f'));
        Assert.That(transformedTemplate.Data[2, 2], Is.EqualTo('c'));
    }
}