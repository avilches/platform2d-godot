using System;
using System.Reflection;

namespace Betauer.Tools.Reflection {
    public class MethodFastGetter : IGetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Func<object?, object> _getValue;
        private readonly FastMethodInfo _fastMethodInfo;
        private readonly string? _toString;

        public object? GetValue(object instance) => _getValue(instance);

        public MethodFastGetter(MethodInfo methodInfo) {
            if (!IsValid(methodInfo)) throw new ArgumentException("Getter method must have 0 parameters and return non void type");
            MemberInfo = methodInfo;
            Type = methodInfo.ReturnType;
            Name = methodInfo.Name;
            _fastMethodInfo = new FastMethodInfo(methodInfo);
            _getValue = (instance) => _fastMethodInfo.Invoke(instance);
            #if DEBUG
                _toString = $"Method: {(methodInfo.IsPrivate ? "private" : "public")} {methodInfo.ReturnType.Name} {Name}()";
            #endif                
        }

        public override string ToString() => _toString ?? base.ToString();

        public static bool IsValid(MemberInfo memberInfo) =>
            memberInfo is MethodInfo methodInfo && methodInfo.GetParameters().Length == 0 && methodInfo.ReturnType != typeof(void);

    }
}