using System.Threading.Tasks;
using Betauer.Memory;
using Betauer.TestRunner;
using Godot;
using NUnit.Framework;

namespace Betauer.Animation.Tests {
    public class NodeTest : Node {
        [SetUp]
        public void RemoveWarning() {
            DisposeTools.ShowWarningOnShutdownDispose = false;
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

        public async Task<Node2D> CreateNode2D() {
            Node2D node2D = new Node2D();
            node2D.Position = new Vector2(100, 100);
            AddChild(node2D);
            await this.AwaitIdleFrame();
            return node2D;
        }

        public async Task<Node> CreateNode() {
            Node node = new Node();
            AddChild(node);
            await this.AwaitIdleFrame();
            return node;
        }

        public async Task<Label> CreateLabel(int width = 100) {
            Label control = new Label();
            control.RectPosition = new Vector2(100, 100);
            control.RectSize = new Vector2(width, width);
            AddChild(control);
            await this.AwaitIdleFrame();
            return control;
        }
    }
}