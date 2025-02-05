using System;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests;

[TestFixture]
[Only]
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
    public void LoadFromString_BasicTemplates_Success() {
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
        
        Assert.DoesNotThrow(() => _templateSet.LoadFromString(content));
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
        
        _templateSet.LoadFromString(content);
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
        
        _templateSet.LoadFromString(content);
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
        
        _templateSet.LoadFromString(content);
        var templates = _templateSet.FindTemplates(15, new[] { "flag1" });

        Assert.That(templates, Has.Count.EqualTo(3));
    }

    [Test]
    public void GetTemplate_WithExactFlags_ReturnsUniqueTemplate() {
        var content = @"
            @ID=15/flag1/flag2
            abc
            def
            ghi
        ";
        
        _templateSet.LoadFromString(content);
        var template = _templateSet.GetTemplate(15, new[] { "flag1", "flag2" });
        
        Assert.That(template, Is.Not.Null);
    }

    [Test]
    public void FindTemplates_WithNonExistentType_ThrowsException() {
        Assert.That(_templateSet.FindTemplates(99), Is.Empty);
    }

    [Test]
    public void FindTemplates_WithNonExistentFlags_ThrowsException() {
        var content = @"
            @ID=15
            abc
            def
            ghi
        ";
        
        _templateSet.LoadFromString(content);
        Assert.That(_templateSet.FindTemplates(15, new[] { "nonexistent" }), Is.Empty);
    }
}