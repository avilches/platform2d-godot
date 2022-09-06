using Godot;

namespace Betauer.Animation.Tween {
    public class AnimationContext<TProperty> {
        public readonly Node Target;
        public readonly TProperty InitialValue;
        public readonly float Duration;

        public TProperty Value { get; private set; }

        public AnimationContext(Node target, TProperty initialValue, float duration) {
            Target = target;
            InitialValue = initialValue;
            Duration = duration;
        }

        // Used by the tween to update the value in the context, then c
        internal void UpdateValue(IProperty<TProperty> property, TProperty value) {
            Value = value;
            property.SetValue(this);
        }
    }
}