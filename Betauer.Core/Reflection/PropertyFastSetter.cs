using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Betauer.Reflection {
    public class PropertyFastSetter : ISetter {
        public Type Type { get; }
        public string Name { get; }
        public MemberInfo MemberInfo { get; }
        private readonly Action<object, object> _setValue;
        private readonly string? _toString;

        public void SetValue(object instance, object value) => _setValue(instance, value);

        public PropertyFastSetter(PropertyInfo propertyInfo) {
            MemberInfo = propertyInfo;
            Type = propertyInfo.PropertyType;
            Name = propertyInfo.Name;
            _setValue = CreateLambdaSetter(propertyInfo); // This is the slow version of property.SetValue;
            #if DEBUG
            _toString = "Property " + Type.Name + " " + Name + " { " +
                        (propertyInfo.GetMethod.IsPrivate ? "private" : "public") + " get; " +
                        (propertyInfo.SetMethod.IsPrivate ? "private" : "public") + " set; }";
            #endif           
        }

        public override string ToString() => _toString ?? base.ToString();
        
        public static Action<object, object> CreateLambdaSetter(PropertyInfo propertyInfo) {
            if (!propertyInfo.CanWrite || propertyInfo.SetMethod == null) {
                throw new Exception("Property " + propertyInfo.Name + " can't be readonly");
            }
            var instanceParam = Expression.Parameter(typeof(object));
            var valueParam = Expression.Parameter(typeof(object));
            var body = Expression.Call
            (Expression.Convert(instanceParam, propertyInfo.DeclaringType),
                propertyInfo.SetMethod,
                Expression.Convert(valueParam, propertyInfo.PropertyType));
            return (Action<object, object>)Expression.Lambda(body, instanceParam, valueParam).Compile();
        }
    }
}