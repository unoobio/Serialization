namespace Serialization.Reflection.Interfaces
{
    internal interface IPropertyDelegator
    {
        string PropertyName { get; init; }

        Type ValueType { get; }

        object? Get(object instance);

        void Set(object instance, object value);
    }
}
