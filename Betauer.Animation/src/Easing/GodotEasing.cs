using System;
using Godot;

namespace Betauer.Animation.Easing {
    public class GodotEasing : IEasing {
        public readonly Tween.EaseType EaseType;
        public readonly Tween.TransitionType TransitionType;

        public string Name { get; }

        internal GodotEasing(Tween.TransitionType transitionType, Tween.EaseType easeType) {
            Name = $"{transitionType}{easeType}";
            TransitionType = transitionType;
            EaseType = easeType;
        }

        public float GetY(float t) {
            return EaseType switch {
                Tween.EaseType.In => EasingFunctions.EaseIn(TransitionType, t),
                Tween.EaseType.Out => EasingFunctions.EaseOut(TransitionType, t),
                Tween.EaseType.InOut => EasingFunctions.EaseInOut(t, TransitionType),
                Tween.EaseType.OutIn => EasingFunctions.EaseOutIn(t, TransitionType),
            };
        }
    }
}