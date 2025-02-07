using System.Linq;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.GridTemplate;

[TestFixture]
public class AttributeParserTests {
    [Test]
    public void Parse_ValidNames_StartingWithLetters() {
        var input = "tag1 my-tag user_name name1=\"Jo'hn\" age-group=30 counter_1=42 name2='Ju\\'anito \"@banana' name3=Hermoso tag4 name4=\"\\\"\"";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "tag1", "my-tag", "user_name", "tag4" }));
        Assert.That(result.Attributes["age-group"], Is.EqualTo(30));
        Assert.That(result.Attributes["counter_1"], Is.EqualTo(42));
        Assert.That(result.Attributes["name1"], Is.EqualTo("Jo'hn"));
        Assert.That(result.Attributes["name2"], Is.EqualTo("Ju'anito \"@banana"));
        Assert.That(result.Attributes["name3"], Is.EqualTo("Hermoso"));
        Assert.That(result.Attributes["name4"], Is.EqualTo("\""));
    }

    [Test]
    public void Parse_ValidNames_StartingWithNumbers() {
        var input = "1tag 2-tag 3_tag 42tag 1name=\"John\" 2.version=3";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "1tag", "2-tag", "3_tag", "42tag" }));
        Assert.That(result.Attributes["1name"], Is.EqualTo("John"));
        Assert.That(result.Attributes["2.version"], Is.EqualTo(3));
    }

    [Test]
    public void Parse_AllowTagDuplicates() {
        var input = "tag1 tag1,Tag1,tag1 Tag1";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "tag1", "Tag1" }));
    }

    [Test]
    public void Parse_Fail_Attribute_Duplicates() {
        var input = "att1=1 att1=2";
        Assert.Throws<AttributeParser.ParseException>(() => AttributeParser.Parse(input),
            "Should throw when duplicate attributes are present");
    }

    [Test]
    public void Parse_Fail_Attribute_Duplicates_IgnoreCase() {
        var input = "att=1 ATT=2";

        var result = AttributeParser.Parse(input);
        Assert.That(result.Attributes["att"], Is.EqualTo(1));
        Assert.That(result.Attributes["ATT"], Is.EqualTo(2));


        Assert.Throws<AttributeParser.ParseException>(() => AttributeParser.Parse(input, true),
            "Should throw when duplicate attributes are present");
    }

    [Test]
    public void Parse_Fail_Attribute_Duplicates_IgnoreCase_Lowercase() {
        var input = "att=1 ATT=2";

        Assert.Throws<AttributeParser.ParseException>(() => AttributeParser.Parse(input, true),
            "Should throw when duplicate attributes are present");
    }

    [Test]
    public void Parse_All_Lowercase() {
        var input = "TAG pePE=1";
        var result = AttributeParser.Parse(input, true);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "tag" }));
        Assert.That(result.Attributes["pepe"], Is.EqualTo(1));
    }

    [Test]
    public void Parse_Unclosed() {
        // Test unclosed double quotes
        var input1 = "1tag 2-tag 3_tag 42tag 1name=\"John Banana";
        Assert.Throws<AttributeParser.ParseException>(() => AttributeParser.Parse(input1),
            "Should throw for unclosed double quotes");

        // Test unclosed single quotes
        var input2 = "1tag 2-tag 3_tag 42tag 1name='John Banana";
        Assert.Throws<AttributeParser.ParseException>(() => AttributeParser.Parse(input2),
            "Should throw for unclosed single quotes");
    }

    [Test]
    public void Parse_ValidNames_StartingWithSpecialChars() {
        var input = "_tag -tag +tag @tag #tag &tag /tag _name=\"John\" @version=3";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "_tag", "-tag", "+tag", "@tag", "#tag", "&tag", "/tag" }));
        Assert.That(result.Attributes["_name"], Is.EqualTo("John"));
        Assert.That(result.Attributes["@version"], Is.EqualTo(3));
    }

    [Test]
    public void Parse_ValidNames_WithDotsInMiddle() {
        var input = "user.name tag.1 version.2.0 first.name=\"John\" version.number=3.14";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "user.name", "tag.1", "version.2.0" }));
        Assert.That(result.Attributes["first.name"], Is.EqualTo("John"));
        Assert.That(result.Attributes["version.number"], Is.EqualTo(3.14f));
    }

    [Test]
    public void Parse_Numbers() {
        var input = "int1=1 int2=+2 int3=-3 float1=1.1 float2=+2.1 float3=-3.1 string=2.3.3";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Attributes["int1"], Is.EqualTo(1));
        Assert.That(result.Attributes["int2"], Is.EqualTo(2));
        Assert.That(result.Attributes["int3"], Is.EqualTo(-3));
        Assert.That(result.Attributes["float1"], Is.EqualTo(1.1f));
        Assert.That(result.Attributes["float2"], Is.EqualTo(2.1f));
        Assert.That(result.Attributes["float3"], Is.EqualTo(-3.1f));
        Assert.That(result.Attributes["string"], Is.EqualTo("2.3.3"));
    }

    [Test]
    public void Parse_ValidNames_ComplexCombinations() {
        var input = "@user.name #tag.1 &version.2.0 @first.name=\"John\" #version.1=3.14 path/to/file";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] { "@user.name", "#tag.1", "&version.2.0", "path/to/file" }));
        Assert.That(result.Attributes["@first.name"], Is.EqualTo("John"));
        Assert.That(result.Attributes["#version.1"], Is.EqualTo(3.14f));
    }

    [Test]
    public void Parse_Names_WithCommas() {
        var result = AttributeParser.Parse("name=\"Jo,hn\" tag1,tag2 tag3 tag4, TAG5,, ,");
        Assert.That(result.Attributes["name"], Is.EqualTo("Jo,hn")); // Las comas en valores están permitidas
        Assert.That(result.Tags, Is.EquivalentTo(new[] { "tag1", "tag2", "tag3", "tag4", "TAG5" }));

        Assert.Throws<AttributeParser.ParseException>(() => AttributeParser.Parse("user,name=\"John\""),
            $"Should throw for invalid input with commas");
    }

    [Test]
    public void Parse_InvalidNames_EmptyOrWhitespace_Comma_SingleQuote_DoubleQuote() {
        var invalidInputs = new[] {
            "=",
            "==",
            "'",
            "''",
            "'''",
            "' '",
            "\"",
            "\"\"",
            "\" \"",
            "\"\"\"",
            "=\"John\"", // Nombre vacío
            " =\"John\"", // Solo espacio
            "\t=\"John\"", // Solo tab
            "\"\"=\"John\"" // Comillas vacías
        };

        foreach (var input in invalidInputs) {
            Assert.Throws<AttributeParser.ParseException>(() =>
                    AttributeParser.Parse(input),
                $"Should throw for empty or whitespace name: {input}");
        }
    }

    [Test]
    public void Parse_AcceptAnySymbolExceptComma_SingleQuote_DoubleQuote_Equals() {
        var input = "@player/1 / enemy# enemy## enemy#boss.1 .=Sigma  #=Sigma user.name=\"John Doe\" hit-points=100 is_active=true #special.tag &custom@tag path//to/file.txt version.1.0 +tag _tag";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Tags, Is.EquivalentTo(new[] {
            "@player/1",
            "/",
            "enemy#",
            "enemy##",
            "enemy#boss.1",
            "#special.tag",
            "&custom@tag",
            "path//to/file.txt",
            "version.1.0",
            "+tag",
            "_tag"
        }));

        Assert.That(result.Attributes["user.name"], Is.EqualTo("John Doe"));
        Assert.That(result.Attributes["."], Is.EqualTo("Sigma"));
        Assert.That(result.Attributes["#"], Is.EqualTo("Sigma"));
        Assert.That(result.Attributes["hit-points"], Is.EqualTo(100));
        Assert.That(result.Attributes["is_active"], Is.EqualTo(true));
    }

    [Test]
    public void Parse_ValuesWithSpecialChars() {
        var input = "path=\"/usr/local/bin\" url=\"http://example.com\" symbols=\"@#$%^&*\" math=\"2+2=4\" tag=\"a.b.c\"";
        var result = AttributeParser.Parse(input);

        Assert.That(result.Attributes["path"], Is.EqualTo("/usr/local/bin"));
        Assert.That(result.Attributes["url"], Is.EqualTo("http://example.com"));
        Assert.That(result.Attributes["symbols"], Is.EqualTo("@#$%^&*"));
        Assert.That(result.Attributes["math"], Is.EqualTo("2+2=4"));
        Assert.That(result.Attributes["tag"], Is.EqualTo("a.b.c"));
    }

    [Test]
    public void Parse_WithComments() {
        // Test basic comments
        var input1 = "tag1 name=\"John\" # This is a comment tag2 name2=\"Jane\"";
        var result1 = AttributeParser.Parse(input1);
        Assert.That(result1.Tags, Is.EquivalentTo(new[] { "tag1" }));
        Assert.That(result1.Attributes["name"], Is.EqualTo("John"));
        Assert.That(result1.Attributes.Count, Is.EqualTo(1));

        // Test // style comments
        var input2 = "tag1 name=\"John\" // This is a comment tag2 name2=\"Jane\"";
        var result2 = AttributeParser.Parse(input2);
        Assert.That(result2.Tags, Is.EquivalentTo(new[] { "tag1" }));
        Assert.That(result2.Attributes["name"], Is.EqualTo("John"));
        Assert.That(result1.Attributes.Count, Is.EqualTo(1));

        // Test comment-only lines
        var input3 = "# This is a comment";
        var result3 = AttributeParser.Parse(input3);
        Assert.That(result3.Tags, Is.Empty);
        Assert.That(result3.Attributes, Is.Empty);

        // Test // comment-only lines
        var input4 = "// This is a comment";
        var result4 = AttributeParser.Parse(input4);
        Assert.That(result4.Tags, Is.Empty);
        Assert.That(result4.Attributes, Is.Empty);
    }

    [Test]
    public void Parse_CommentsInStrings() {
        // Test # in string values
        var input1 = "tag1 comment=\"This #is not a comment\"";
        var result1 = AttributeParser.Parse(input1);
        Assert.That(result1.Tags, Is.EquivalentTo(new[] { "tag1" }));
        Assert.That(result1.Attributes["comment"], Is.EqualTo("This #is not a comment"));

        // Test // in string values
        var input2 = "tag1 comment=\"This // is not a comment\"";
        var result2 = AttributeParser.Parse(input2);
        Assert.That(result2.Tags, Is.EquivalentTo(new[] { "tag1" }));
        Assert.That(result2.Attributes["comment"], Is.EqualTo("This // is not a comment"));
    }
}