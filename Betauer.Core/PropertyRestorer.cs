using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Betauer {
    public abstract class Restorer {
        public bool HasSavedState { get; private set; } = false;

        public Restorer Save() {
            DoSave();
            HasSavedState = true;
            return this;
        }

        /// <summary>
        /// Warning: always all to Restore() in a idle_frame, not in a tween/signal callback
        /// <code>
        /// await this.AwaitIdleFrame();
        /// restorer.Restore();
        /// </code>
        /// </summary>
        /// <returns></returns>
        public Restorer Restore() {
            if (!HasSavedState) {
                // GD.PushWarning("Restoring without saving before");
                return this;
            }
            DoRestore();
            return this;
        }

        protected abstract void DoSave();
        protected abstract void DoRestore();
    }

    public static class RestoreExtensions {
        public static Restorer CreateRestorer(this Node node) {
            return node switch {
                Node2D node2D => new Node2DRestorer(node2D),
                Control control => new ControlRestorer(control),
                _ => DummyRestorer.Instance
            };
        }
    }

    public class MultiRestorer : Restorer {
        private readonly List<Restorer> _restorers;

        public MultiRestorer(IEnumerable<Node> nodes) {
            _restorers = nodes.Select(node => node.CreateRestorer()).ToList();
        }

        public MultiRestorer(params Node[] nodes) {
            _restorers = nodes.Select(node => node.CreateRestorer()).ToList();
        }

        protected override void DoSave() {
            foreach (var restorer in _restorers) {
                restorer.Save();
            }
        }

        protected override void DoRestore() {
            foreach (var restorer in _restorers) {
                restorer.Restore();
            }
        }
    }

    public class Node2DRestorer : Restorer {
        private readonly Node2D _node2D;
        private readonly SpritePivotOffsetRestorer? _pivotOffsetRestorer;
        private Color _modulate;
        private Color _selfModulate;
        private Transform2D _transform;
        private float _rotation; // TODO: this field doesn't have tests

        public Node2DRestorer(Node2D node2D) {
            _node2D = node2D;
            _pivotOffsetRestorer = _node2D is Sprite sprite ? new SpritePivotOffsetRestorer(sprite) : null;
        }

        protected override void DoSave() {
            _pivotOffsetRestorer?.Save();
            _modulate = _node2D.Modulate;
            _selfModulate = _node2D.SelfModulate;
            _transform = _node2D.Transform;
            _rotation = _node2D.Rotation;
        }

        protected override void DoRestore() {
            _pivotOffsetRestorer?.Restore();
            _node2D.Modulate = _modulate;
            _node2D.SelfModulate = _selfModulate;
            _node2D.Transform = _transform;
            _node2D.Rotation = _rotation;
        }
    }

    public class ControlRestorer : Restorer {
        private readonly Control _control;
        private Color _modulate;
        private Color _selfModulate;
        private Vector2 _position;
        private Vector2 _scale;
        private float _rotation;
        private Vector2 _rectPivotOffset;

        public ControlRestorer(Control control) {
            _control = control;
        }

        protected override void DoSave() {
            _modulate = _control.Modulate;
            _selfModulate = _control.SelfModulate;
            _position = _control.RectPosition;
            _scale = _control.RectScale;
            _rotation = _control.RectRotation;
            _rectPivotOffset = _control.RectPivotOffset;
            // GD.Print("Save _modulate:" + _modulate);
            // GD.Print("Save _selfModulate:" + _selfModulate);
            // GD.Print("Save _position:" + _position);
            // GD.Print("Save _scale:" + _scale);
            // GD.Print("Save _rotation:" + _rotation);
            // GD.Print("Save _rectPivotOffset:" + _rectPivotOffset);
        }

        protected override void DoRestore() {
            // GD.Print("Restore _modulate from " + _control.Modulate +" to: "+ _modulate);
            // GD.Print("Restore _selfModulate from " + _control.SelfModulate +" to: "+ _selfModulate);
            // GD.Print("Restore _position from " + _control.RectPosition +" to: "+ _position);
            // GD.Print("Restore _scale from " + _control.RectScale +" to: "+ _scale);
            // GD.Print("Restore _rotation from " + _control.RectRotation +" to: "+ _rotation);
            // GD.Print("Restore _rectPivotOffset from " + _control.RectPivotOffset +" to: "+ _rectPivotOffset);
            _control.Modulate = _modulate;
            _control.SelfModulate = _selfModulate;
            _control.RectPosition = _position;
            _control.RectScale = _scale;
            _control.RectRotation = _rotation;
            _control.RectPivotOffset = _rectPivotOffset;
        }
    }

    public class RectPivotOffsetRestorer : Restorer {
        private readonly Control _node;
        private Vector2 _originalRectPivotOffset;

        public RectPivotOffsetRestorer(Control node) {
            _node = node;
        }

        protected override void DoSave() {
            _originalRectPivotOffset = _node.RectPivotOffset;
        }

        protected override void DoRestore() {
            _node.RectPivotOffset = _originalRectPivotOffset;
        }
    }

    public class SpritePivotOffsetRestorer : Restorer {
        private readonly Sprite _node;
        private Vector2 _offset;
        private Vector2 _globalPosition;

        public SpritePivotOffsetRestorer(Sprite node) {
            _node = node;
        }

        protected override void DoSave() {
            _offset = _node.Offset;
            _globalPosition = _node.GlobalPosition;
        }

        protected override void DoRestore() {
            _node.Offset = _offset;
            _node.GlobalPosition = _globalPosition;
        }
    }

    public class DummyRestorer : Restorer {
        public static readonly Restorer Instance = new DummyRestorer();

        private DummyRestorer() {
        }

        protected override void DoSave() {
        }

        protected override void DoRestore() {
        }
    }
}