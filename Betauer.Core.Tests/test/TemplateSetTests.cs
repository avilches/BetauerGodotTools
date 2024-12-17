using System;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

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
    public void LoadTemplates_BasicTemplates_Success() {
        var content = @"
            @ID=15
            abc
            def
            ghi
            @ID=15/special
            xyz
            uvw
            rst
        ";
        
        Assert.DoesNotThrow(() => _templateSet.LoadTemplates(content));
    }

    [Test]
    public void FindTemplates_ByType_ReturnsAllMatchingTemplates() {
        var content = @"
            @ID=15
            abc
            def
            ghi
            @ID=15/special
            xyz
            uvw
            rst
        ";
        
        _templateSet.LoadTemplates(content);
        var templates = _templateSet.FindTemplates(15);
        
        Assert.That(templates, Has.Count.EqualTo(2));
    }

    [Test]
    public void FindTemplates_WithRequiredFlags_ReturnsMatchingTemplates() {
        var content = @"
            @ID=15
            abc
            def
            ghi
            @ID=15/special/extra
            xyz
            uvw
            rst
        ";
        
        _templateSet.LoadTemplates(content);
        var templates = _templateSet.FindTemplates(15, new[] { "special" });
        
        Assert.That(templates, Has.Count.EqualTo(1));
    }

    [Test]
    public void FindTemplates_WithRequiredAndOptionalFlags_ReturnsOrderedTemplates() {
        var content = @"
            @ID=15/flag1
            abc
            def
            ghi
            @ID=15/flag1/flag2
            xyz
            uvw
            rst
            @ID=15/flag1/flag2/flag3
            123
            456
            789
        ";
        
        _templateSet.LoadTemplates(content);
        var templates = _templateSet.FindTemplates(15, 
            new[] { "flag1" }, 
            new[] { "flag2", "flag3" });
        
        Assert.That(templates, Has.Count.EqualTo(3));
        // Should be ordered by number of matching optional flags
        Assert.That(templates[0].ToString(), Contains.Substring("123")); // Has both optional flags
        Assert.That(templates[1].ToString(), Contains.Substring("xyz")); // Has one optional flag
        Assert.That(templates[2].ToString(), Contains.Substring("abc")); // Has no optional flags
    }

    [Test]
    public void GetTemplate_WithExactFlags_ReturnsUniqueTemplate() {
        var content = @"
            @ID=15/flag1/flag2
            abc
            def
            ghi
        ";
        
        _templateSet.LoadTemplates(content);
        var template = _templateSet.GetTemplate(15, new[] { "flag1", "flag2" });
        
        Assert.That(template, Is.Not.Null);
    }

    [Test]
    public void GetTemplate_WithMultipleMatches_ThrowsException() {
        var content = @"
            @ID=15/flag1/flag3
            abc
            def
            ghi
            @ID=15/flag1/flag2
            xyz
            uvw
            rst
        ";
        
        _templateSet.LoadTemplates(content);
        Assert.Throws<ArgumentException>(() => 
            _templateSet.GetTemplate(15, new[] { "flag1" }));
    }

    [Test]
    public void FindTemplates_WithNonExistentType_ThrowsException() {
        Assert.Throws<ArgumentException>(() => _templateSet.FindTemplates(99));
    }

    [Test]
    public void FindTemplates_WithNonExistentFlags_ThrowsException() {
        var content = @"
            @ID=15
            abc
            def
            ghi
        ";
        
        _templateSet.LoadTemplates(content);
        Assert.Throws<ArgumentException>(() => 
            _templateSet.FindTemplates(15, new[] { "nonexistent" }));
    }
}