using System.Threading.Tasks;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Animation.Tests {
    public partial class NodeTest : Node {
        public async Task<Sprite2D> CreateSprite(int width = 100) {
            Sprite2D sprite = new Sprite2D();
            sprite.Position = new Vector2(100, 100);
            // var gradientTexture = new GradientTexture();
            var imageTexture = new ImageTexture();
            imageTexture.SetSizeOverride(new Vector2i(width, width));
            sprite.Texture = imageTexture;
            AddChild(sprite);
            await this.AwaitProcessFrame();
            return sprite;
        }

        public async Task<Node2D> CreateNode2D() {
            Node2D node2D = new Node2D();
            node2D.Position = new Vector2(100, 100);
            AddChild(node2D);
            await this.AwaitProcessFrame();
            return node2D;
        }

        public async Task<Node> CreateNode() {
            Node node = new Node();
            AddChild(node);
            await this.AwaitProcessFrame();
            return node;
        }

        public async Task<Label> CreateLabel(int width = 100) {
            Label control = new Label();
            control.Position = new Vector2(100, 100);
            control.Size = new Vector2(width, width);
            AddChild(control);
            await this.AwaitProcessFrame();
            return control;
        }
    }
}