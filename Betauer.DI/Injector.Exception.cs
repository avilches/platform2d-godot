using System;

namespace Betauer.DI {
    public abstract class InjectException : Exception {
        public readonly object Instance;

        public InjectException(string message, object instance) : base(message) {
            Instance = instance;
        }
    }

    public class InjectFieldException : InjectException {
        public readonly string Name;

        public InjectFieldException(string name, object instance, string message) : base(message, instance) {
            Name = name;
        }
    }
}