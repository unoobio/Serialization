using Serialization.Reflection.Interfaces;
using System.Collections.Concurrent;
using System.Reflection;

namespace Serialization.Reflection
{
    internal class TypeDelegatorProvider
    {
        private static ConcurrentDictionary<string, ITypeDelegator> _typeDeligators = new();

        public static ITypeDelegator GetTypeDelegator(Type type)
        {
            if (!_typeDeligators.TryGetValue(type.FullName!, out ITypeDelegator typeDeligator))
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                Type genericType = typeof(TypeDelegator<>).MakeGenericType(type);
                ConstructorInfo? constructor = genericType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(TypeInfo) });
                typeDeligator = (ITypeDelegator)constructor!.Invoke(new[] { typeInfo });
                _typeDeligators.TryAdd(typeInfo.FullName!, typeDeligator);
            }

            return typeDeligator;
        }
    }
}
