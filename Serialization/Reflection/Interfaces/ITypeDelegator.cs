namespace Serialization.Reflection.Interfaces
{
    internal interface ITypeDelegator
    {
        Dictionary<string, IPropertyDelegator> PropertyDelegators { get; }

        Dictionary<string, IFieldDelegator> FieldDelegators { get; }

        object CreateInstance();
    }
}
