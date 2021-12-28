using Godot;

namespace Betauer.Animation {
    /**
     * Special thanks to Alessandro Senese (Ceceppa)
     *
     * All the tricks to set pivots in Control nodes and create fake pivot in Sprite nodes are possible because
     * of his work in the wonderful library Anima: https://github.com/ceceppa/anima
     *
     * Thank you man! :)
     * Code from: https://github.com/ceceppa/anima/blob/master/addons/anima/utils/node_properties.gd
     */
    public static class PropertyTools {
        public static Vector2 GetSpriteSize(this Sprite sprite) => sprite.Texture.GetSize() * sprite.Scale;

        public static float GetOutOfScreenRight(this Node node) {
            return node is CanvasItem canvas ? canvas.GetViewport().Size.x : node.GetTree().Root.Size.x;
        }

        public static float GetOutOfScreenBottom(this Node node) {
            return node is CanvasItem canvas ? canvas.GetViewport().Size.y : node.GetTree().Root.Size.y;
        }

        public static float GetOutOfScreenLeft(this Node node) {
            var offset = node switch {
                Control control => control.RectSize.x,
                Sprite sprite => sprite.GetSpriteSize().x,
                _ => 0
            };
            return -(node.GetOutOfScreenRight() + offset);
        }

        public static float GetOutOfScreenTop(this Node node) {
            var offset = node switch {
                Control control => control.RectSize.y,
                Sprite sprite => sprite.GetSpriteSize().y,
                _ => 0
            };
            return -(node.GetOutOfScreenBottom() + offset);
        }


        public interface IRestorer {
            public void Rollback();
        }

        private class RectPivotOffsetRestorer : IRestorer {
            private readonly Control _node;
            private readonly Vector2 _originalRectPivotOffset;

            public RectPivotOffsetRestorer(Control node) {
                _node = node;
                _originalRectPivotOffset = node.RectPivotOffset;
            }

            public void Rollback() {
                _node.RectPivotOffset = _originalRectPivotOffset;
            }
        }

        private class SpritePivotOffsetRestorer : IRestorer {
            private readonly Sprite _node;
            private readonly Vector2 _offset;
            private readonly Vector2 _globalPosition;

            public SpritePivotOffsetRestorer(Sprite node) {
                _node = node;
                _offset = node.Offset;
                _globalPosition = node.GlobalPosition;
            }

            public void Rollback() {
                _node.Offset = _offset;
                _node.GlobalPosition = _globalPosition;
            }
        }

        private class DummyRestorer : IRestorer {
            public static readonly IRestorer Instance = new DummyRestorer();

            public void Rollback() {
            }
        }

        public static IRestorer SetControlPivot(this Control control, Vector2 offset) {
            var restorer = new RectPivotOffsetRestorer(control);
            control.RectPivotOffset = offset;
            return restorer;
        }

        public static IRestorer SetSpritePivot(this Sprite sprite, Vector2 offset) {
            var restorer = new SpritePivotOffsetRestorer(sprite);
            var position = sprite.GlobalPosition;
            sprite.Offset = offset;
            sprite.GlobalPosition = position - sprite.Offset;
            return restorer;
        }

        // TODO: TopRight, CenterRight, CenterLeft.
        // TODO: Create a Visual UI to check the behavior (rotate with different origin)
        public static IRestorer SetRotateOriginToTopCenter(this Node node) {
            return node switch {
                // node.set_pivot_offset(Vector2(size.x / 2, 0))
                Control control => control.SetControlPivot(new Vector2(control.RectSize.x / 2, 0)),
                // node.offset = Vector2(0, size.y / 2)
                Sprite sprite => sprite.SetSpritePivot(new Vector2(0, GetSpriteSize(sprite).y / 2)),
                _ => DummyRestorer.Instance
            };
        }

        public static IRestorer SetRotateOriginToTopLeft(this Node node) {
            return node switch {
                // node.set_pivot_offset(Vector2(0, 0))
                Control control => SetControlPivot(control, Vector2.Zero),
                // node.offset = Vector2(size.x / 2, 0)
                Sprite sprite => sprite.SetSpritePivot(new Vector2(GetSpriteSize(sprite).x / 2, 0)),
                _ => DummyRestorer.Instance
            };
        }

        public static IRestorer SetRotateOriginToCenter(this Node node) {
            return node switch {
                // node.set_pivot_offset(size / 2)
                Control control => SetControlPivot(control, control.RectSize / 2),
                _ => DummyRestorer.Instance
            };
        }

        public static IRestorer SetRotateOriginToBottomCenter(this Node node) {
            switch (node) {
                // node.set_pivot_offset(Vector2(size.x / 2, size.y / 2))
                case Control control: {
                    var size = control.RectSize;
                    return SetControlPivot(control, new Vector2(size.x / 2, size.y / 2));
                }
                // node.offset = Vector2(0, -size.y / 2)
                case Sprite sprite:
                    return sprite.SetSpritePivot(new Vector2(0, -GetSpriteSize(sprite).y / 2));
                default:
                    return DummyRestorer.Instance;
            }
        }

        public static IRestorer SetRotateOriginToBottomLeft(this Node node) {
            switch (node) {
                // node.set_pivot_offset(Vector2(0, size.y))
                case Control control:
                    return SetControlPivot(control, new Vector2(0, control.RectSize.y));
                // node.offset = Vector2(size.x / 2, size.y)
                case Sprite sprite: {
                    var size = GetSpriteSize(sprite);
                    return sprite.SetSpritePivot(new Vector2(size.x / 2, size.y));
                }
                default:
                    return DummyRestorer.Instance;
            }
        }

        public static IRestorer SetRotateOriginToBottomRight(this Node node) {
            switch (node) {
                // node.set_pivot_offset(Vector2(size.x, size.y / 2))
                case Control control: {
                    var size = control.RectSize;
                    return SetControlPivot(control, new Vector2(size.x, size.y / 2));
                }
                // node.offset = Vector2(-size.x / 2, size.y / 2)
                case Sprite sprite: {
                    var size = GetSpriteSize(sprite);
                    return sprite.SetSpritePivot(new Vector2(-size.x / 2, size.y / 2));
                }
                default:
                    return DummyRestorer.Instance;
            }
        }
    }
}