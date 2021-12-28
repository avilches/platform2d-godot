using System;
using Godot;

namespace Betauer.Animation {
    internal static partial class Templates {
        private const float RotateDuration = 1f; // Animate.css: 1f

        // https://github.com/animate-css/animate.css/tree/main/source/rotating_entrances

        internal static TweenSequenceTemplate RotateIn() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 200, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInDownLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInDownRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInUpLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 45, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateInUpRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, -45, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 0.0f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 0f)
                .KeyframeTo(1.0f, 1f)
                .EndAnimate()
                .BuildTemplate();
        }

        internal static TweenSequenceTemplate RotateOut() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToCenter())
                .KeyframeTo(1.00f, 200f)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateOutDownLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateOutDownRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateOutUpLeft() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomLeft())
                .KeyframeTo(1.00f, 45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
        internal static TweenSequenceTemplate RotateOutUpRight() {
            return TweenSequenceBuilder.Create()
                .SetDuration(RotateDuration)
                .AnimateKeys(property: Property.RotateCenter)
                .KeyframeTo(0.00f, 0.0f, null, node => node.SetRotateOriginToBottomRight())
                .KeyframeTo(1.00f, -45)
                .EndAnimate()
                .Parallel()
                .AnimateKeys(property: Property.Opacity)
                .KeyframeTo(0.0f, 1f)
                .KeyframeTo(1.0f, 0f)
                .EndAnimate()
                .BuildTemplate();
        }
    }
}