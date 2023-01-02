namespace Serialization.Reflection.Interfaces
{
    internal interface IFieldDelegator
    {
        string FieldName { get; }

        Type ValueType { get; }

        object? Get(object instance);

        void Set(object instance, object value);
    }
}
