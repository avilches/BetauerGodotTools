using Betauer.Core.DataMath;
using Betauer.Core.PCG.GridTemplate;
using Betauer.TestRunner;
using NUnit.Framework;

namespace Betauer.Core.Tests.GridTemplate;

[TestFixture]
public class TemplateTest {
    private Template CreateTemplateWithAllFlags() {
        var template = new Template() { Body = new Array2D<char>(2, 2) };

        // Non-directional attributes
        template.SetAttribute("normal", "value");

        // Left-Right flags
        template.SetAttribute(DirectionFlag.Left, "value", "left");
        template.SetAttribute(DirectionFlag.Right, "value", "right");

        // Up-Down flags
        template.SetAttribute(DirectionFlag.Up, "value", "up");
        template.SetAttribute(DirectionFlag.Down, "value", "down");

        // Diagonal flags
        template.SetAttribute(DirectionFlag.UpLeft, "value", "upleft");
        template.SetAttribute(DirectionFlag.UpRight, "value", "upright");
        template.SetAttribute(DirectionFlag.DownLeft, "value", "bottomleft");
        template.SetAttribute(DirectionFlag.DownRight, "value", "bottomright");

        return template;
    }

    [Test]
    public void AttributesWithAlias() {
        // Arrange
        var template = new Template();

        // Act
        template.SetAttribute(DirectionFlag.Up, "value", "up");
        Assert.That(template.GetAttribute("dir:U:value"), Is.EqualTo("up"));

        template.SetAttribute(DirectionFlag.Down, "value", "down");
        Assert.That(template.GetAttribute("dir:D:value"), Is.EqualTo("down"));
    }

    [Test]
    public void Transform_Rotate90_ShouldRotateClockwise() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.Rotate90);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Flags are rotated 90 degrees clockwise
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("left")); // Up becomes Left
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("up")); // Right becomes Up
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("right")); // Down becomes Right
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("down")); // Left becomes Down

            // Diagonal flags are also rotated
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("bottomleft")); // UpLeft becomes BottomLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("upleft")); // UpRight becomes UpLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("upright")); // BottomRight becomes UpRight
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("bottomright")); // BottomLeft becomes BottomRight
        });
    }

    [Test]
    public void Transform_Rotate180_ShouldRotateDouble() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.Rotate180);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Flags are rotated 180 degrees
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("down")); // Up becomes Down
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("left")); // Right becomes Left
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("up")); // Down becomes Up
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("right")); // Left becomes Right

            // Diagonal flags are also rotated
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("bottomright")); // UpLeft becomes BottomRight
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("bottomleft")); // UpRight becomes BottomLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("upleft")); // BottomRight becomes UpLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("upright")); // BottomLeft becomes UpRight
        });
    }

    [Test]
    public void Transform_RotateMinus90_ShouldRotateCounterClockwise() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.RotateMinus90);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Flags are rotated 90 degrees counter-clockwise
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("right")); // Up becomes Right
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("down")); // Right becomes Down
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("left")); // Down becomes Left
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("up")); // Left becomes Up

            // Diagonal flags are also rotated
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("upright")); // UpLeft becomes UpRight
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("bottomright")); // UpRight becomes BottomRight
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("bottomleft")); // BottomRight becomes BottomLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("upleft")); // BottomLeft becomes UpLeft
        });
    }

    [Test]
    public void Transform_FlipH_ShouldFlipHorizontally() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.FlipH);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Vertical flags remain unchanged
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("up"));
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("down"));

            // Horizontal flags are flipped
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("left")); // Right becomes Left
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("right")); // Left becomes Right

            // Diagonal flags are flipped horizontally
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("upright")); // UpLeft becomes UpRight
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("upleft")); // UpRight becomes UpLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("bottomleft")); // BottomRight becomes BottomLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("bottomright")); // BottomLeft becomes BottomRight
        });
    }

    [Test]
    public void Transform_FlipV_ShouldFlipVertically() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.FlipV);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Horizontal flags remain unchanged
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("left"));
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("right"));

            // Vertical flags are flipped
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("down")); // Up becomes Down
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("up")); // Down becomes Up

            // Diagonal flags are flipped vertically
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("bottomleft")); // UpLeft becomes BottomLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("bottomright")); // UpRight becomes BottomRight
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("upright")); // BottomRight becomes UpRight
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("upleft")); // BottomLeft becomes UpLeft
        });
    }

    [Test]
    public void Transform_FlipDiagonal_ShouldFlipByMainDiagonal() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.FlipDiagonal);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Cardinal flags are rotated by primary diagonal
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("left")); // Up becomes Left
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("up")); // Left becomes Up
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("right")); // Down becomes Right
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("down")); // Right becomes Down

            // Diagonal flags: UpLeft and DownRight stay the same (they're on the diagonal)
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("upleft")); // UpLeft stays
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("bottomright")); // DownRight stays

            // Other diagonal flags are swapped
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("bottomleft")); // UpRight becomes DownLeft
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("upright")); // DownLeft becomes UpRight
        });
    }

    [Test]
    public void Transform_FlipDiagonalSecondary_ShouldFlipBySecondaryDiagonal() {
        // Arrange
        var template = CreateTemplateWithAllFlags();

        // Act
        var transformed = template.Transform(Transformations.Type.FlipDiagonalSecondary);

        // Assert
        Assert.Multiple(() => {
            // Non-directional attributes remain unchanged
            Assert.That(transformed.GetAttribute("normal"), Is.EqualTo("value"));

            // Cardinal flags are rotated along the secondary diagonal
            Assert.That(transformed.GetAttribute(DirectionFlag.Up, "value"), Is.EqualTo("right")); // Up becomes Right
            Assert.That(transformed.GetAttribute(DirectionFlag.Right, "value"), Is.EqualTo("up")); // Right becomes Up
            Assert.That(transformed.GetAttribute(DirectionFlag.Down, "value"), Is.EqualTo("left")); // Down becomes Left
            Assert.That(transformed.GetAttribute(DirectionFlag.Left, "value"), Is.EqualTo("down")); // Left becomes Down

            // Diagonal flags: UpRight and DownLeft stay the same (they're on the diagonal)
            Assert.That(transformed.GetAttribute(DirectionFlag.UpRight, "value"), Is.EqualTo("upright")); // UpRight stays
            Assert.That(transformed.GetAttribute(DirectionFlag.DownLeft, "value"), Is.EqualTo("bottomleft")); // DownLeft stays

            // Other diagonal flags are swapped
            Assert.That(transformed.GetAttribute(DirectionFlag.UpLeft, "value"), Is.EqualTo("bottomright")); // UpLeft becomes DownRight
            Assert.That(transformed.GetAttribute(DirectionFlag.DownRight, "value"), Is.EqualTo("upleft")); // DownRight becomes UpLeft
        });
    }

    [Test]
    public void Transform_FlipDiagonal_ShouldTransformDirectionFlagsAndBody() {
        // Arrange
        var template = new Template {
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.UpRight),
            Body = new Array2D<char>(new[,] {
                { '1', '2' },
                { '3', '4' }
            })
        };

        // Act
        var transformed = template.Transform(Transformations.Type.FlipDiagonal);

        // Assert
        Assert.Multiple(() => {
            // DirectionFlags: Up|Right|UpRight -> Left|Down|DownLeft
            Assert.That(transformed.DirectionFlags, Is.EqualTo((byte)(DirectionFlag.Left | DirectionFlag.Down | DirectionFlag.DownLeft)));

            // Body should be flipped by main diagonal (top-left to bottom-right):
            // [1 2]    [1 3]
            // [3 4] -> [2 4]
            var expected = new[,] {
                { '1', '3' },
                { '2', '4' }
            };

            Assert.That(transformed.Body.Width, Is.EqualTo(2));
            Assert.That(transformed.Body.Height, Is.EqualTo(2));
            for (var y = 0; y < transformed.Body.Height; y++) {
                for (var x = 0; x < transformed.Body.Width; x++) {
                    Assert.That(transformed.Body[y, x], Is.EqualTo(expected[y, x]), $"Cell at [{y}, {x}] is incorrect");
                }
            }
        });
    }

    [Test]
    public void Transform_FlipDiagonalSecondary_ShouldTransformDirectionFlagsAndBody() {
        // Arrange
        var template = new Template {
            DirectionFlags = (byte)(DirectionFlag.Up | DirectionFlag.Right | DirectionFlag.UpRight),
            Body = new Array2D<char>(new[,] {
                { '1', '2' },
                { '3', '4' }
            })
        };

        // Act
        var transformed = template.Transform(Transformations.Type.FlipDiagonalSecondary);

        // Assert
        Assert.Multiple(() => {
            // DirectionFlags: Up|Right|UpRight -> Right|Up|UpRight (UpRight stays the same as it's on the diagonal)
            Assert.That(transformed.DirectionFlags, Is.EqualTo((byte)(DirectionFlag.Right | DirectionFlag.Up | DirectionFlag.UpRight)));

            // Body should be flipped by secondary diagonal (top-right to bottom-left):
            // [1 2]    [4 2]
            // [3 4] -> [3 1]
            var expected = new[,] {
                { '4', '2' },
                { '3', '1' }
            };

            Assert.That(transformed.Body.Width, Is.EqualTo(2));
            Assert.That(transformed.Body.Height, Is.EqualTo(2));
            for (var y = 0; y < transformed.Body.Height; y++) {
                for (var x = 0; x < transformed.Body.Width; x++) {
                    Assert.That(transformed.Body[y, x], Is.EqualTo(expected[y, x]), $"Cell at [{y}, {x}] is incorrect");
                }
            }
        });
    }
}