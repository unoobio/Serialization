using System.Reflection;
using System.Reflection.Emit;
using Serialization.Reflection.Interfaces;

namespace Serialization.Reflection
{
    internal class TypeDelegator<TInstance> : ITypeDelegator
    {
        private readonly TypeInfo _typeInfo;
        
        public TypeDelegator(TypeInfo typeInfo)
        {
            _typeInfo = typeInfo ?? throw new ArgumentNullException(nameof(typeInfo));
            this.Init();
        }

        public Dictionary<string, IPropertyDelegator> PropertyDelegators { get; private set; }

        public Dictionary<string, IFieldDelegator> FieldDelegators { get; private set; }

        private Func<TInstance> ConstructorDelegator { get; set; }

        public object CreateInstance()
        {
            if (this.ConstructorDelegator == null)
                this.CreateConstructorDelegate();

            return this.ConstructorDelegator();
        }

        private void Init()
        {          
            this.PropertyDelegators = new Dictionary<string, IPropertyDelegator>();
            PropertyInfo[] properiesInfo = _typeInfo.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var propertyInfo in properiesInfo)
            {
                var genericType = typeof(PropertyDelegator<,>).MakeGenericType(typeof(TInstance), propertyInfo.PropertyType);
                var constructor = genericType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(PropertyInfo) });
                IPropertyDelegator properyDelegator = (IPropertyDelegator)constructor!.Invoke(new[] { propertyInfo });
                this.PropertyDelegators.Add(propertyInfo.Name, properyDelegator);
            }

            this.FieldDelegators = new Dictionary<string, IFieldDelegator>();
            FieldInfo[] fieldsInfo = _typeInfo.GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fieldsInfo)
            {
                var genericType = typeof(FieldDelegator<,>).MakeGenericType(typeof(TInstance), fieldInfo.FieldType);
                var constructor = genericType.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[] { typeof(FieldInfo) });
                IFieldDelegator fieldDelegator = (IFieldDelegator)constructor!.Invoke(new[] { fieldInfo });
                this.FieldDelegators.Add(fieldInfo.Name, fieldDelegator);
            }

            this.CreateConstructorDelegate();
        }

        private void CreateConstructorDelegate()
        {
            ConstructorInfo constructorInfo = _typeInfo.GetConstructor(BindingFlags.Public | BindingFlags.Instance, new Type[0])
                    ?? throw new Exception($"Public empty constructor does not found in type \"{_typeInfo.Name}\".");

            DynamicMethod dynamic = new DynamicMethod(string.Empty,
                typeof(TInstance),
                Type.EmptyTypes,
                typeof(TInstance));
            ILGenerator il = dynamic.GetILGenerator();

            il.DeclareLocal(typeof(TInstance));
            il.Emit(OpCodes.Newobj, constructorInfo);
            il.Emit(OpCodes.Stloc_0);
            il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);

            this.ConstructorDelegator = (Func<TInstance>)dynamic.CreateDelegate(typeof(Func<TInstance>));
        }
    }
}
