using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Tests {
    [TestFixture]
    public class RestorerTests : Node {

        [Test]
        public void PivotControlRestoreTests() {
            var control = new Control();
            var original = new Vector2(2f, 3f);

            control.RectPivotOffset = original;
            control.Modulate = new Color(0f,1f,0f);
            control.SelfModulate = new Color(0f,1f,0f);
            control.RectScale = original;
            control.RectPosition = original;
            control.RectRotation = 3f;

            var status = control.CreateRestorer().Save();
            control.RectPivotOffset = Vector2.Zero;
            control.Modulate = new Color(0.1f,0.2f,0.3f);
            control.SelfModulate = new Color(0.1f,0.2f,0.3f);
            control.RectScale *= 0.2f;
            control.RectPosition += Vector2.One;
            control.RectRotation *= 3f;

            status.Restore();

            Assert.That(control.RectPivotOffset, Is.EqualTo(original));
            Assert.That(control.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(control.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(control.RectScale, Is.EqualTo(original));
            Assert.That(control.RectPosition, Is.EqualTo(original));
            Assert.That(control.RectRotation, Is.EqualTo(3f));
        }

        [Test]
        public async Task PivotSpriteRestoreTests() {
            var sprite = await CreateSprite();
            var original = new Vector2(2f, 3f);

            sprite.Offset = original;
            sprite.GlobalPosition = original;

            sprite.Modulate = new Color(0f,1f,0f);
            sprite.SelfModulate = new Color(0f,1f,0f);
            sprite.Scale = Vector2.One;
            sprite.Position = original;
            sprite.Rotation = 3f;

            var status = sprite.CreateRestorer().Save();
            sprite.Offset = Vector2.Zero;
            sprite.GlobalPosition = Vector2.Zero;
            sprite.Modulate = new Color(0.1f,0.2f,0.3f);
            sprite.SelfModulate = new Color(0.1f,0.2f,0.3f);
            sprite.Scale *= 0.2f;
            sprite.Position += Vector2.One;
            sprite.Rotation *= 3f;

            status.Restore();

            Assert.That(sprite.Offset, Is.EqualTo(original));
            Assert.That(sprite.GlobalPosition, Is.EqualTo(original));
            Assert.That(sprite.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(sprite.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(sprite.Scale, Is.EqualTo(Vector2.One));
            Assert.That(sprite.Position, Is.EqualTo(original));
            Assert.That(sprite.Rotation, Is.EqualTo(3f));
        }

        [Test]
        public async Task Node2DRestoreTests() {
            var sprite = new Node2D();
            AddChild(sprite);
            await this.AwaitIdleFrame();
            var original = new Vector2(2f, 3f);

            sprite.GlobalPosition = original;

            sprite.Modulate = new Color(0f,1f,0f);
            sprite.SelfModulate = new Color(0f,1f,0f);
            sprite.Scale = Vector2.One;
            sprite.Position = original;
            sprite.Rotation = 3f;

            var status = sprite.CreateRestorer().Save();
            sprite.GlobalPosition = Vector2.Zero;
            sprite.Modulate = new Color(0.1f,0.2f,0.3f);
            sprite.SelfModulate = new Color(0.1f,0.2f,0.3f);
            sprite.Scale *= 0.2f;
            sprite.Position += Vector2.One;
            sprite.Rotation *= 3f;

            status.Restore();

            Assert.That(sprite.GlobalPosition, Is.EqualTo(original));
            Assert.That(sprite.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(sprite.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
            Assert.That(sprite.Scale, Is.EqualTo(Vector2.One));
            Assert.That(sprite.Position, Is.EqualTo(original));
            Assert.That(sprite.Rotation, Is.EqualTo(3f));
        }

        public async Task<Sprite> CreateSprite(int width = 100) {
            Sprite sprite = new Sprite();
            sprite.Position = new Vector2(100, 100);
            // var gradientTexture = new GradientTexture();
            var imageTexture = new ImageTexture();
            imageTexture.SetSizeOverride(new Vector2(width, width));
            sprite.Texture = imageTexture;
            AddChild(sprite);
            await this.AwaitIdleFrame();
            return sprite;
        }



    }
}