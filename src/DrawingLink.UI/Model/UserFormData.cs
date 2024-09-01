using System;
using System.Collections.Generic;

namespace DrawingLink.UI.Model
{
    public class UserFormData
    {
        public string DefinitionPath { get; }
        private readonly Dictionary<string, string> _stringPerFieldName;
        private readonly Dictionary<string, int> _intPerFieldName;
        private readonly Dictionary<string, double> _doublePerFieldName;

        public UserFormData(string definitionPath, IReadOnlyList<string> strings, IReadOnlyList<double> doubles, IReadOnlyList<int> integers)
        {
            DefinitionPath = definitionPath ?? throw new ArgumentNullException(nameof(definitionPath));
            _stringPerFieldName = SetupDictionary(strings);
            _intPerFieldName = SetupDictionary(integers);
            _doublePerFieldName = SetupDictionary(doubles);
        }

        private Dictionary<string, T> SetupDictionary<T>(IEnumerable<T> values)
        {
            var counter = 0;
            var type = GetTypeName(typeof(T));

            var result = new Dictionary<string, T>();
            foreach (var item in values)
            {
                result[$"{type}_{counter}"] = item;
                counter++;
            }

            return result;
        }

        private string GetTypeName(Type type)
        {
            if (type == typeof(int))
                return "int";
            else if (type == typeof(double))
                return "double";
            else
                return "string";
        }

        public bool TryGetStringValue(string fieldName, out string result)
        {
            result = string.Empty;
            if (!_stringPerFieldName.ContainsKey(fieldName))
                return false;

            result = _stringPerFieldName[fieldName];
            if (string.IsNullOrWhiteSpace(result))
                return false;

            return true;
        }

        public bool TryGetIntValue(string fieldName, out int result)
        {
            result = int.MinValue;
            if (!_intPerFieldName.ContainsKey(fieldName))
                return false;

            result = _intPerFieldName[fieldName];
            if (result == int.MinValue)
                return false;

            return true;
        }

        public bool TryGetDoubleValue(string fieldName, out double result)
        {
            result = double.MinValue;
            if (!_doublePerFieldName.ContainsKey(fieldName))
                return false;

            result = _doublePerFieldName[fieldName];
            if (result == double.MinValue)
                return false;

            return true;
        }
    }
}
