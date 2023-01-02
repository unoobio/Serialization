using System.Reflection;
using System.Reflection.Emit;
using Serialization.Reflection.Interfaces;

namespace Serialization.Reflection
{
    internal class FieldDelegator<TInstance, TFieldValue> : IFieldDelegator
        where TFieldValue : IConvertible
    {
        private readonly FieldInfo _fieldInfo;

        public FieldDelegator(FieldInfo fieldInfo)
        {
            _fieldInfo = fieldInfo ?? throw new ArgumentNullException(nameof(fieldInfo));
            this.FieldName = _fieldInfo.Name;
            this.ValueType = typeof(TFieldValue);
            this.Init();
        }

        public Type ValueType { get; init; }

        public string FieldName { get; init; }

        private Func<TInstance, TFieldValue> GetDelegator { get; set; }

        private Action<TInstance, TFieldValue> SetDelegator { get; set; }


        public void Set(object instance, object value)
            => this.SetDelegator((TInstance)instance, (TFieldValue)value);

        public object? Get(object instance)
            => this.GetDelegator((TInstance)instance);

        private void Init()
        {
            this.GetDelegator = this.CreateGetter();
            this.SetDelegator = this.CreateSetter();
        }

        private Func<TInstance, TFieldValue> CreateGetter()
        {
            string methodName = _fieldInfo.ReflectedType!.FullName + ".get_" + _fieldInfo.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, typeof(TFieldValue), new Type[1] { typeof(TInstance) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            if (_fieldInfo.IsStatic)
            {
                gen.Emit(OpCodes.Ldsfld, _fieldInfo);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, _fieldInfo);
            }
            gen.Emit(OpCodes.Ret);
            return (Func<TInstance, TFieldValue>)setterMethod.CreateDelegate(typeof(Func<TInstance, TFieldValue>));
        }

        private Action<TInstance, TFieldValue> CreateSetter()
        {
            string methodName = _fieldInfo.ReflectedType!.FullName + ".set_" + _fieldInfo.Name;
            DynamicMethod setterMethod = new DynamicMethod(methodName, null, new Type[2] { typeof(TInstance), typeof(TFieldValue) }, true);
            ILGenerator gen = setterMethod.GetILGenerator();
            if (_fieldInfo.IsStatic)
            {
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stsfld, _fieldInfo);
            }
            else
            {
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Stfld, _fieldInfo);
            }
            gen.Emit(OpCodes.Ret);
            return (Action<TInstance, TFieldValue>)setterMethod.CreateDelegate(typeof(Action<TInstance, TFieldValue>));
        }
    }
}
