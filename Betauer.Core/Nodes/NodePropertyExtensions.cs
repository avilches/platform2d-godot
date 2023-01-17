using Betauer.Core.Restorer;
using Godot;

namespace Betauer.Core.Nodes; 

/**
 * Special thanks to Alessandro Senese (Ceceppa)
 *
 * All the tricks to set pivots in Control nodes and create fake pivot in Sprite2D nodes are possible because
 * of his work in the wonderful library Anima: https://github.com/ceceppa/anima
 *
 * Thank you man! :)
 * Code from: https://github.com/ceceppa/anima/blob/master/addons/anima/utils/node_properties.gd
 */
public static class NodePropertyExtensions {
    public static Vector2 GetSpriteSize(this Sprite2D sprite) => sprite.Texture.GetSize() * sprite.Scale;

    public static float GetOutOfScreenRight(this Node node) {
        return node is CanvasItem canvas ? canvas.GetViewportRect().Size.x : node.GetTree().Root.Size.x;
    }

    public static float GetOutOfScreenBottom(this Node node) {
        return node is CanvasItem canvas ? canvas.GetViewportRect().Size.y : node.GetTree().Root.Size.y;
    }

    public static float GetOutOfScreenLeft(this Node node) {
        var offset = node switch {
            Control control => control.Size.x,      
            Sprite2D sprite => sprite.GetSpriteSize().x,
            _ => 0
        };
        return -(node.GetOutOfScreenRight() + offset);
    }

    public static float GetOutOfScreenTop(this Node node) {
        var offset = node switch {
            Control control => control.Size.y,
            Sprite2D sprite => sprite.GetSpriteSize().y,
            _ => 0
        };
        return -(node.GetOutOfScreenBottom() + offset);
    }

    public static Restorer.Restorer SetControlPivot(this Control control, Vector2 offset) {
        var restorer = new PivotOffsetRestorer(control);
        restorer.Save();
        control.PivotOffset = offset;
        return restorer;
    }

    public static Restorer.Restorer SetSpritePivot(this Sprite2D sprite, Vector2 offset) {
        var restorer = new SpritePivotOffsetRestorer(sprite);
        restorer.Save();
        var position = sprite.GlobalPosition;
        sprite.Offset = offset;
        sprite.GlobalPosition = position - sprite.Offset;
        return restorer;
    }

    // TODO: TopRight, CenterRight, CenterLeft.
    // TODO: Create a Visual UI to check the behavior (rotate with different origin)
    public static Restorer.Restorer SetRotateOriginToTopCenter(this Node node) {
        return node switch {
            // node.set_pivot_offset(Vector2(size.x / 2, 0))
            Control control => control.SetControlPivot(new Vector2(control.Size.x / 2, 0)),
            // node.offset = Vector2(0, size.y / 2)
            Sprite2D sprite => sprite.SetSpritePivot(new Vector2(0, GetSpriteSize(sprite).y / 2)),
            _ => DummyRestorer.Instance
        };
    }

    public static Restorer.Restorer SetRotateOriginToTopLeft(this Node node) {
        return node switch {
            // node.set_pivot_offset(Vector2(0, 0))
            Control control => SetControlPivot(control, Vector2.Zero),
            // node.offset = Vector2(size.x / 2, 0)
            Sprite2D sprite => sprite.SetSpritePivot(new Vector2(GetSpriteSize(sprite).x / 2, 0)),
            _ => DummyRestorer.Instance
        };
    }

    public static Restorer.Restorer SetRotateOriginToCenter(this Node node) {
        return node switch {
            // node.set_pivot_offset(size / 2)
            Control control => SetControlPivot(control, control.Size / 2),
            _ => DummyRestorer.Instance
        };
    }

    public static Restorer.Restorer SetRotateOriginToBottomCenter(this Node node) {
        switch (node) {
            // node.set_pivot_offset(Vector2(size.x / 2, size.y / 2))
            case Control control: {
                var size = control.Size;
                return SetControlPivot(control, new Vector2(size.x / 2, size.y / 2));
            }
            // node.offset = Vector2(0, -size.y / 2)
            case Sprite2D sprite:
                return sprite.SetSpritePivot(new Vector2(0, -GetSpriteSize(sprite).y / 2));
            default:
                return DummyRestorer.Instance;
        }
    }

    public static Restorer.Restorer SetRotateOriginToBottomLeft(this Node node) {
        switch (node) {
            // node.set_pivot_offset(Vector2(0, size.y))
            case Control control:
                return SetControlPivot(control, new Vector2(0, control.Size.y));
            // node.offset = Vector2(size.x / 2, size.y)
            case Sprite2D sprite: {
                var size = GetSpriteSize(sprite);
                return sprite.SetSpritePivot(new Vector2(size.x / 2, size.y));
            }
            default:
                return DummyRestorer.Instance;
        }
    }

    public static Restorer.Restorer SetRotateOriginToBottomRight(this Node node) {
        switch (node) {
            // node.set_pivot_offset(Vector2(size.x, size.y / 2))
            case Control control: {
                var size = control.Size;
                return SetControlPivot(control, new Vector2(size.x, size.y / 2));
            }
            // node.offset = Vector2(-size.x / 2, size.y / 2)
            case Sprite2D sprite: {
                var size = GetSpriteSize(sprite);
                return sprite.SetSpritePivot(new Vector2(-size.x / 2, size.y / 2));
            }
            default:
                return DummyRestorer.Instance;
        }
    }
}