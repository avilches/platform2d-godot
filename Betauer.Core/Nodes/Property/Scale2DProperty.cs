using System;
using Godot;

namespace Betauer.Nodes.Property {
    public class Scale2DProperty : Property<Vector2> {
        public override Vector2 GetValue(Node node) {
            return node switch {
                Node2D node2D => node2D.Scale,
                Control control => control.RectScale,
                _ => throw new NodeNotCompatibleWithPropertyException($"Not Scale2D property for node type {node.GetType()}")
            };
        }

        public override void SetValue(Node node, Vector2 value) {
            if (node is Node2D node2D) node2D.Scale = value;
            else if (node is Control control) control.RectScale = value;
            else throw new NodeNotCompatibleWithPropertyException($"Not Scale2D property for node type {node.GetType()}");
        }

        public override bool IsCompatibleWith(Node node) {
            return node is Control || node is Node2D;
        }

        public override string ToString() {
            return "Scale2DProperty<Vector2>(node2D: \"Scale\", control: \"RectScale\")";
        }
    }
}