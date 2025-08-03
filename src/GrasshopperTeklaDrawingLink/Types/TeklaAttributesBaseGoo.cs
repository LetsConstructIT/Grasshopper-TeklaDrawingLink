using GH_IO.Serialization;
using Grasshopper.Kernel.Types;
using GTDrawingLink.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GTDrawingLink.Types
{
    public abstract class TeklaAttributesBaseGoo<T> : GH_Goo<T> where T : class
    {
        public override bool IsValid => true;

        public override string TypeDescription => $"Tekla attributes: {nameof(T)}";

        public override string TypeName => typeof(T).ToShortString();

        public TeklaAttributesBaseGoo()
        {
        }

        public TeklaAttributesBaseGoo(T attributes) : base(attributes)
        {
        }

        public override IGH_Goo Duplicate()
        {
            throw new NotImplementedException();
        }

        public override bool CastFrom(object source)
        {
            if (source is T)
            {
                Value = source as T;
                return true;
            }
            return base.CastFrom(source);
        }

        public override string ToString()
        {
            if (Value == null)
                return "No value";

            return ReflectionHelper.GetPropertiesWithValues(Value);
        }

        public override bool Write(GH_IWriter writer)
        {
            if (Value != null)
            {
                var propertyValues = new Dictionary<string, ValueWithType>();
                SplitPropertiesIntoPrimitives(Value, ref propertyValues);

                foreach (var propertyValue in propertyValues)
                {
                    if (propertyValue.Value.Type == typeof(int) ||
                        propertyValue.Value.Type == typeof(Enum))
                        writer.SetInt32(propertyValue.Key, (int)propertyValue.Value.Value);
                    else if (propertyValue.Value.Type == typeof(double))
                        writer.SetDouble(propertyValue.Key, (double)propertyValue.Value.Value);
                    else if (propertyValue.Value.Type == typeof(string))
                        writer.SetString(propertyValue.Key, (string)propertyValue.Value.Value);
                    else if (propertyValue.Value.Type == typeof(bool))
                        writer.SetBoolean(propertyValue.Key, (bool)propertyValue.Value.Value);
                }
            }

            return base.Write(writer);
        }

        private void SplitPropertiesIntoPrimitives(object inputObject, ref Dictionary<string, ValueWithType> outPropertyValues)
        {
            if (inputObject == null)
                return;

            var type = inputObject.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public).OrderBy(p => p.Name))
            {
                if (!property.CanWrite)
                    continue;

                var propType = property.PropertyType;

                var propertyValue = property.GetValue(inputObject);
                var key = GetKey(type, property);

                if (propType == typeof(int))
                    outPropertyValues[key] = ValueWithType.Integer(propertyValue);
                else if (propType == typeof(double))
                    outPropertyValues[key] = ValueWithType.Double(propertyValue);
                else if (propType == typeof(string))
                    outPropertyValues[key] = ValueWithType.String(propertyValue);
                else if (propType == typeof(bool))
                    outPropertyValues[key] = ValueWithType.Boolean(propertyValue);
#if API2024 || API2025
                else if (propType == typeof(Tekla.Structures.Drawing.TeklaDrawingColor))
                    outPropertyValues[key] = ValueWithType.Color(((Tekla.Structures.Drawing.TeklaDrawingColor)propertyValue).RGBColor.ToArgb());
#endif
                else if (propType.IsEnum)
                    outPropertyValues[key] = ValueWithType.Enum(propertyValue);
                else
                    SplitPropertiesIntoPrimitives(propertyValue, ref outPropertyValues);
            }
        }

        public override bool Read(GH_IReader reader)
        {
            var neededType = typeof(T);
            Value = (T)Activator.CreateInstance(neededType, nonPublic: true);

            if (reader.ItemCount > 1) // there is always one property set by GH, TypeName
                FillProperties(Value, reader);

            return base.Read(reader);
        }

        private string GetKey(Type type, PropertyInfo propertyInfo)
            => $"{type.FullName}_{propertyInfo.Name}";

        private void FillProperties(object inputObject, GH_IReader reader)
        {
            var type = inputObject.GetType();
            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (!property.CanWrite)
                    continue;

                var propType = property.PropertyType;
                var key = GetKey(type, property);

                if (propType == typeof(int))
                {
                    var value = 0;
                    if (reader.TryGetInt32(key, ref value))
                        property.SetValue(inputObject, value);
                }
                else if (propType == typeof(double))
                {
                    var value = 0.0;
                    if (reader.TryGetDouble(key, ref value))
                        property.SetValue(inputObject, value);
                }
                else if (propType == typeof(string))
                {
                    var value = string.Empty;
                    if (reader.TryGetString(key, ref value))
                        property.SetValue(inputObject, value);
                }
                else if (propType == typeof(bool))
                {
                    var value = false;
                    if (reader.TryGetBoolean(key, ref value))
                        property.SetValue(inputObject, value);
                }
#if API2024 || API2025
                else if (propType == typeof(Tekla.Structures.Drawing.TeklaDrawingColor))
                {
                    var value = 0;
                    if (reader.TryGetInt32(key, ref value))
                    {
                        var color = System.Drawing.Color.FromArgb(value);
                        var teklaColor = new Tekla.Structures.Drawing.TeklaDrawingColor(color);
                        property.SetValue(inputObject, teklaColor);
                    }
                }
#endif
                else if (propType.IsEnum)
                {
                    var value = 0;
                    if (reader.TryGetInt32(key, ref value))
                        property.SetValue(inputObject, value);
                }
                else
                    FillProperties(property.GetValue(inputObject), reader);
            }
        }

        private class ValueWithType
        {
            public object Value { get; }
            public Type Type { get; }

            private ValueWithType(object value, Type type)
            {
                Value = value ?? throw new ArgumentNullException(nameof(value));
                Type = type ?? throw new ArgumentNullException(nameof(type));
            }

            public static ValueWithType Integer(object value)
                => new ValueWithType(value, typeof(int));

            public static ValueWithType Double(object value)
                => new ValueWithType(value, typeof(double));

            public static ValueWithType Boolean(object value)
                => new ValueWithType(value, typeof(bool));

            public static ValueWithType String(object value)
                => new ValueWithType(value, typeof(string));

            public static ValueWithType Color(int argb)
                => new ValueWithType(argb, typeof(int));

            public static ValueWithType Enum(object value)
                => new ValueWithType(value, typeof(Enum));
        }
    }
}
