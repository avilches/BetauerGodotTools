using Godot;

namespace Game.Tools {
    [Tool]
    public class Collored2D : CollisionShape2D {
        private Color _color = Colors.White;
        private bool _enabled_editor = true;
        private bool _enabled_game = true;

        [Export]
        Color color {
            get => _color;
            set {
                _color = value;
                Update();
            }
        }

        [Export]
        bool enabled_editor {
            get => _enabled_editor;
            set {
                _enabled_editor = value;
                Update();
            }
        }

        [Export]
        bool enabled_game {
            get => _enabled_game;
            set {
                _enabled_game = value;
                Update();
            }
        }

        public override void _Draw() {
            if (Engine.EditorHint) {
                if (!_enabled_editor) return;
            } else {
                if (!_enabled_game) return;
            }

            var shape2D = Shape;
            switch (shape2D) {
                case RectangleShape2D r: {
                    var rExtents = r.Extents;
                    DrawRect(new Rect2(-rExtents, rExtents * 2.0f), color);
                    break;
                }
                case CircleShape2D c:
                    DrawCircle(Vector2.Zero, c.Radius, color);
                    break;
                
                case CapsuleShape2D c:
                    var cHeight = c.Height;
                    var cRadius = c.Radius;
                    DrawCircle(new Vector2(0, cHeight / 2F), cRadius, color);
                    DrawCircle(new Vector2(0, -cHeight / 2F), cRadius, color);
                    DrawRect(new Rect2(new Vector2(-cRadius, -cHeight / 2F), new Vector2(cRadius * 2, cHeight)), color);
                    break;
            }
        }
    }
}