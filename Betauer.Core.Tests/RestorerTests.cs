using System.Threading.Tasks;
using Betauer.Core.Nodes.Property;
using Betauer.Core.Restorer;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Core.Tests; 

[TestFixture]
public partial class RestorerTests : Node {

    [Test]
    public async Task PropertyNameRestoreTests() {
        var control = new Control();
        var node2D = new Node2D();
        AddChild(control);
        AddChild(node2D);
        await this.AwaitIdleFrame();
        var original = new Vector2(2f, 2f);
        control.Scale = original;
        node2D.Scale = original;
        Assert.That(control.Scale, Is.EqualTo(original));
        Assert.That(node2D.Scale, Is.EqualTo(original));

        var status = control.CreateRestorer("scale").Add(node2D.CreateRestorer("scale"));
        status.Save();
        control.Scale = Vector2.One;
        node2D.Scale = Vector2.One;;
        Assert.That(control.Scale, Is.EqualTo(Vector2.One));
        Assert.That(node2D.Scale, Is.EqualTo(Vector2.One));

        status.Restore();
        Assert.That(control.Scale, Is.EqualTo(original));
        Assert.That(node2D.Scale, Is.EqualTo(original));
    }
        
    [Test]
    public async Task PropertyRestoreTests() {
        var control = new Control();
        var node2D = new Node2D();
        AddChild(control);
        AddChild(node2D);
        await this.AwaitIdleFrame();
        var original = new Vector2(2f, 2f);
        control.Scale = original;
        node2D.Scale = original;
        Assert.That(control.Scale, Is.EqualTo(original));
        Assert.That(node2D.Scale, Is.EqualTo(original));

        var status = new Node[] { control, node2D }.CreateMultiRestorer(Properties.Scale2D);
        status.Save();
        control.Scale = Vector2.One;
        node2D.Scale = Vector2.One;;
        Assert.That(control.Scale, Is.EqualTo(Vector2.One));
        Assert.That(node2D.Scale, Is.EqualTo(Vector2.One));

        status.Restore();
        Assert.That(control.Scale, Is.EqualTo(original));
        Assert.That(node2D.Scale, Is.EqualTo(original));
    }

    [Test]
    public async Task DefaultControlRestoreTests() {
        var control = new Control();
        AddChild(control);
        await this.AwaitIdleFrame();
        var original = new Vector2(2f, 3f);

        control.PivotOffset = original;
        control.Modulate = new Color(0f,1f,0f);
        control.SelfModulate = new Color(0f,1f,0f);
        control.Scale = original;
        control.Position = original;
        control.Rotation = 3f;

        var status = control.CreateRestorer();
        status.Save();
        control.PivotOffset = Vector2.Zero;
        control.Modulate = new Color(0.1f,0.2f,0.3f);
        control.SelfModulate = new Color(0.1f,0.2f,0.3f);
        control.Scale *= 0.2f;
        control.Position += Vector2.One;
        control.Rotation *= 3f;

        status.Restore();

        Assert.That(control.PivotOffset, Is.EqualTo(original));
        Assert.That(control.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
        Assert.That(control.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
        Assert.That(control.Scale, Is.EqualTo(original));
        Assert.That(control.Position, Is.EqualTo(original));
        Assert.That(control.Rotation, Is.EqualTo(3f));
    }

    [Test]
    public async Task ControlFocusRestoreTests() {
        var container = new HBoxContainer();
        var b1 = new Button();
        var b2 = new Button();
        container.AddChild(b1);
        container.AddChild(b2);
        AddChild(container);
        await this.AwaitIdleFrame();
        b1.GrabFocus();
        await this.AwaitIdleFrame();
        Assert.That(container.GetViewport().GuiGetFocusOwner(), Is.EqualTo(b1));

        var r = container.CreateFocusOwnerRestorer();
        r.Save();
        b2.GrabFocus();
        await this.AwaitIdleFrame();
        Assert.That(container.GetViewport().GuiGetFocusOwner(), Is.EqualTo(b2));

        r.Restore();
        await this.AwaitIdleFrame();
        Assert.That(container.GetViewport().GuiGetFocusOwner(), Is.EqualTo(b1));
    }

    [Test]
    public async Task PivotSpriteRestoreTests() {
        var sprite = await CreateSprite();
        var original = new Vector2(2f, 3f);

        sprite.Hframes = 2;
        sprite.Vframes = 2;
        sprite.Offset = original;
        sprite.GlobalPosition = original;

        sprite.Modulate = new Color(0f,1f,0f);
        sprite.SelfModulate = new Color(0f,1f,0f);
        sprite.Scale = Vector2.One;
        sprite.Position = original;
        sprite.Rotation = 3f;
        sprite.Frame = 2;
        sprite.FlipH = true;
        sprite.FlipV = true;

        var status = sprite.CreateRestorer();
        status.Save();
        sprite.Offset = Vector2.Zero;
        sprite.GlobalPosition = Vector2.Zero;
        sprite.Modulate = new Color(0.1f,0.2f,0.3f);
        sprite.SelfModulate = new Color(0.1f,0.2f,0.3f);
        sprite.Scale *= 0.2f;
        sprite.Position += Vector2.One;
        sprite.Rotation *= 3f;
        sprite.Frame = 0;
        sprite.FlipH = !sprite.FlipH;
        sprite.FlipV = !sprite.FlipV;

        status.Restore();

        Assert.That(sprite.Offset, Is.EqualTo(original));
        Assert.That(sprite.GlobalPosition, Is.EqualTo(original));
        Assert.That(sprite.Modulate, Is.EqualTo(new Color(0f,1f,0f)));
        Assert.That(sprite.SelfModulate, Is.EqualTo(new Color(0f,1f,0f)));
        Assert.That(sprite.Scale, Is.EqualTo(Vector2.One));
        Assert.That(sprite.Position, Is.EqualTo(original));
        Assert.That(sprite.Rotation, Is.EqualTo(3f));
        Assert.That(sprite.Frame, Is.EqualTo(2));
        Assert.That(sprite.FlipH, Is.True);
        Assert.That(sprite.FlipV, Is.True);
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

        var status = sprite.CreateRestorer();
        status.Save();
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

    public async Task<Sprite2D> CreateSprite(int width = 100) {
        Sprite2D sprite = new Sprite2D();
        sprite.Position = new Vector2(100, 100);
        // var gradientTexture = new GradientTexture();
        var imageTexture = new ImageTexture();
        imageTexture.SetSizeOverride(new Vector2i(width, width));
        sprite.Texture = imageTexture;
        AddChild(sprite);
        await this.AwaitIdleFrame();
        return sprite;
    }
}