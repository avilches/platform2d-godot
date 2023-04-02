using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Tools.Reflection.FastImpl; 

public class PropertyFastSetter : ISetter {
    private readonly Action<object, object> _setValue;
    private readonly string? _toString;

    public PropertyFastSetter(PropertyInfo propertyInfo) {
        if (!IsValid(propertyInfo))
            throw new ArgumentException($"PropertyInfo {propertyInfo.Name} doesn't have set", nameof(propertyInfo));
        PropertyInfo = propertyInfo;
        MemberInfo = propertyInfo;
        Type = propertyInfo.PropertyType;
        Name = propertyInfo.Name;
        DeclaringType = propertyInfo.DeclaringType;
        _setValue = CreateLambdaSetter(propertyInfo);
#if DEBUG
        _toString = Type.GetTypeName() + " " + Name +
                    (propertyInfo.GetMethod != null
                        ? propertyInfo.GetMethod.IsPrivate ? " { private get; " : " { public get; "
                        : " { ") +
                    (propertyInfo.SetMethod.IsPrivate ? "private set; }" : "public set; }");
#endif
    }

    public Type Type { get; }
    public string Name { get; }
    public MemberInfo MemberInfo { get; }
    public PropertyInfo PropertyInfo { get; }
    public Type DeclaringType { get; }

    public void SetValue(object instance, object? value) {
        _setValue(instance, value);
    }

    public override string ToString() {
        return _toString ?? base.ToString();
    }

    public static bool IsValid(MemberInfo memberInfo) {
        return memberInfo is PropertyInfo { CanWrite: true } propertyInfo && propertyInfo.SetMethod != null;
    }

    public static Action<object, object> CreateLambdaSetter(PropertyInfo propertyInfo) {
        var instanceParam = Expression.Parameter(typeof(object));
        var valueParam = Expression.Parameter(typeof(object));
        var body = Expression.Call
        (Expression.Convert(instanceParam, propertyInfo.DeclaringType),
            propertyInfo.SetMethod,
            Expression.Convert(valueParam, propertyInfo.PropertyType));
        return (Action<object, object>)Expression.Lambda(body, instanceParam, valueParam).Compile();
    }
}