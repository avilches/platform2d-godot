using System;

namespace Betauer.Core.Easing;

public class InterpolationFunc : IInterpolation {
    public Func<float, float> Function { get; }

    public InterpolationFunc(Func<float, float> function) {
        Function = function;
    }

    public float GetY(float t) => Function.Invoke(t);
}