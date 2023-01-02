using Microsoft.VisualBasic.FileIO;
using Serialization.Reflection;
using Serialization.Reflection.Interfaces;
using System.Linq;
using System.Text;

namespace Serialization.Serializators
{
    /// <summary>
    /// Самописная сериализация через рефлексию.
    /// </summary>
    internal class ReflectionSerializer
    {
        /// <summary>
        /// Сериализует в csv формат.
        /// </summary>
        public static string SerializeToCsvFormat<TInstanse>(IList<TInstanse> objects)
        {
            if (objects.Count < 1)
                throw new ArgumentException($"{nameof(objects)} length must be grater than 0.");
            
            StringBuilder stringBuilder = new StringBuilder();

            ITypeDelegator typeDelegator = TypeDelegatorProvider.GetTypeDelegator(objects[0]!.GetType());

            var classMemberNames = typeDelegator.PropertyDelegators.Keys
                .Concat(typeDelegator.FieldDelegators.Keys);


            stringBuilder.AppendLine(string.Join(",", classMemberNames));

            foreach (TInstanse obj in objects)
            {
                SerializeToCsvFormat(obj, stringBuilder, typeDelegator);
            }

            return stringBuilder.ToString();
        }

        private static void SerializeToCsvFormat<TInstance>(TInstance obj, StringBuilder stringBuilder, ITypeDelegator typeDelegator)
        {
            var values = typeDelegator.PropertyDelegators.Values.Select(propDelegator =>
                    Convert.ChangeType(propDelegator.Get(obj), typeof(string), System.Globalization.CultureInfo.InvariantCulture))
                .Concat(typeDelegator.FieldDelegators.Values.Select(fieldDelegator =>
                    Convert.ChangeType(fieldDelegator.Get(obj), typeof(string), System.Globalization.CultureInfo.InvariantCulture)));

            stringBuilder.AppendLine(string.Join(",", values));
        }

        /// <summary>
        /// Десериализует из csv формата.
        /// </summary>
        public static IList<TInstance> Deserialize<TInstance>(string csv)
        {
            ITypeDelegator typeDelegator = TypeDelegatorProvider.GetTypeDelegator(typeof(TInstance));

            List<TInstance> resultObjects = new List<TInstance>();

            using (TextReader textReader = new StringReader(csv))
            {
                using (TextFieldParser parser = new TextFieldParser(textReader))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    int count = 0;
                    string[]? headerNames = null;
                    while (!parser.EndOfData)
                    {
                        //Process row
                        string[]? fields = parser.ReadFields();
                        if (fields == null)
                            return new List<TInstance>();

                        if (count == 0)
                            headerNames = fields;
                        else
                        {
                            TInstance instance = (TInstance)typeDelegator.CreateInstance();
                            for (int i = 0; i < fields.Length; i++)
                            {
                                if (typeDelegator.PropertyDelegators.TryGetValue(headerNames![i], out IPropertyDelegator? propertyDelegator))
                                {
                                    object value = Convert.ChangeType(fields[i], propertyDelegator.ValueType, System.Globalization.CultureInfo.InvariantCulture);
                                    propertyDelegator.Set(instance, value);
                                }
                                else if (typeDelegator.FieldDelegators.TryGetValue(headerNames![i], out IFieldDelegator? fieldDelegator))
                                {
                                    object value = Convert.ChangeType(fields[i], fieldDelegator.ValueType, System.Globalization.CultureInfo.InvariantCulture);
                                    fieldDelegator.Set(instance, value);
                                }
                            }
                            resultObjects.Add(instance);
                        }

                        count++;
                    }
                }
            }

            return resultObjects;
        }
    }
}
