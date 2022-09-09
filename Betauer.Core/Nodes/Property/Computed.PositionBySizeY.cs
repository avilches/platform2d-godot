using Godot;

namespace Betauer.Nodes.Property {
    public class PositionBySizeY : ComputedProperty<float> {
        private readonly float _initialValue;

        public PositionBySizeY(Node node) : base(node, Properties.PositionY) {
            if (IsCompatibleWith(node)) {
                _initialValue = Properties.PositionY.GetValue(node);
            }
        }

        protected override float ComputeValue(float percent) {
            var size = Node switch {
                Sprite sprite => sprite.GetSpriteSize(),
                Control control => control.RectSize,
            };
            return _initialValue + (size.y * percent);
        }

        public sealed override bool IsCompatibleWith(Node node) {
            return node is Sprite || node is Control;
        }

        public override string ToString() {
            return $"PositionBySizeY.Computed({Property})";
        }
    }
}