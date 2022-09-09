using System;
using System.Reflection;

namespace Betauer.Reflection {
    public interface IGetterSetter : ISetter, IGetter {}
    public interface IGetterSetter<out T> : ISetter<T>, IGetter<T> where T : Attribute {}

    public class FastGetterSetter<T> : FastGetterSetter, IGetterSetter<T> where T : Attribute {
        public T Attribute { get; }

        public FastGetterSetter(MemberInfo member, T attribute) : base(member) {
            Attribute = attribute;
        }
    }

    public class FastGetterSetter : IGetterSetter {
        private readonly IGetter _iGetter;
        private readonly ISetter _iSetter;
        
        public Type Type => _iGetter.Type;
        public string Name => _iGetter.Name;
        public MemberInfo MemberInfo => _iGetter.Type;
        public void SetValue(object instance, object value) => _iSetter.SetValue(instance, value);
        public object GetValue(object instance) => _iGetter.GetValue(instance);
        
        public FastGetterSetter(MemberInfo memberInfo) {
            if (memberInfo is PropertyInfo propertyInfo) {
                _iGetter = new PropertyFastGetter(propertyInfo);
                _iSetter = new PropertyFastSetter(propertyInfo);
            } else if (memberInfo is FieldInfo fieldInfo) {
                _iGetter = new FieldFastGetter(fieldInfo);
                _iSetter = new FieldFastSetter(fieldInfo);
            } else {
                throw new ArgumentException("Member must be PropertyInfo or FieldInfo");
            }
        }
    }
}