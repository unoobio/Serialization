using System.Reflection;
using Serialization.Reflection.Interfaces;

namespace Serialization.Reflection
{
    internal class PropertyDelegator<TInstance, TPropertyValue> : IPropertyDelegator
        where TPropertyValue : IConvertible
    {
        private readonly PropertyInfo _propertyInfo;

        public PropertyDelegator(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            this.PropertyName = _propertyInfo.Name;
            this.ValueType = typeof(TPropertyValue);
            this.Init();
        }

        public Type ValueType { get; private set; }

        public string PropertyName { get; init; }

        private Func<TInstance, TPropertyValue> GetDelegator { get; set; }

        private Action<TInstance, TPropertyValue> SetDelegator { get; set; }

        public void Set(object instance, object value)
            => this.SetDelegator((TInstance)instance, (TPropertyValue)value);

        public object? Get(object instance)
            => this.GetDelegator((TInstance)instance);

        private void Init()
        {
            this.GetDelegator = (Func<TInstance, TPropertyValue>)Delegate.CreateDelegate(
                typeof(Func<TInstance, TPropertyValue>),
                _propertyInfo.GetGetMethod(true)!);

            this.SetDelegator = (Action<TInstance, TPropertyValue>)Delegate.CreateDelegate(
                typeof(Action<TInstance, TPropertyValue>),
                _propertyInfo.GetSetMethod(true)!);
        }
    }
}
