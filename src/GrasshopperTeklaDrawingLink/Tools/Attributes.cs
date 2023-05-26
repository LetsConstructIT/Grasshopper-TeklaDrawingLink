using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace GTDrawingLink.Tools
{
    public class Attributes : SortedDictionary<string, object>
    {
        public Attributes()
        {
        }

        public static Attributes Parse<T>(Dictionary<string, T> attributeCollection)
        {
            Attributes attributes = new Attributes();
            foreach (KeyValuePair<string, T> keyValuePair in attributeCollection)
                attributes.Add(keyValuePair.Key, keyValuePair.Value);

            return attributes;
        }

        public static Attributes Parse(string rowCollection)
        {
            if (rowCollection == null)
                return null;

            Attributes attributes = new Attributes();
            foreach (string item in from r in Regex.Split(rowCollection, "\\r\\n|\\r|\\n").SelectMany((string r) => Regex.Split(r, "(?<!\\G([^\"]*\"[^\"]*\")*(?:[^\"]+\"[^\"]*));|;\\s?$"))
                                    select r.Trim() into r
                                    where !string.IsNullOrWhiteSpace(r)
                                    select r)
            {
                string[] array = item.Split(new char[1] { ' ' }, 2);
                if (array.Length < 2 || string.IsNullOrWhiteSpace(array[0]) || string.IsNullOrWhiteSpace(array[1]))
                    throw new FormatException("Incorrect entry '" + item.Trim() + "'. Use syntax 'USER_FIELD_1 \"mytext\"' or 'MyInt 1' or 'MyFloat 3.0'.");
                
                string text = array[0].Trim();
                string text2 = array[1].Trim();

                try
                {
                    if (text2.Length > 1 && text2[0] == '"' && text2[text2.Length - 1] == '"')
                    {
                        text2 = text2.Substring(1, text2.Length - 2);
                        attributes.Add(text, text2);
                        continue;
                    }

                    if (int.TryParse(text2, out var result))
                    {
                        attributes.Add(text, result);
                        continue;
                    }

                    if (double.TryParse(text2, NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var result2))
                    {
                        attributes.Add(text, result2);
                        continue;
                    }

                    string text3 = "Attribute '" + text + "' value '" + text2 + "' could not be parsed as a string, integer or float. If it's a string, use quotation marks around the value. If you use quotation marks within the string value (e.g. to indicate inches), put the attribute on a separate row.";
                    if (text2.IndexOf(',') > -1)
                        text3 += " If the value is a float, use a dot as the decimal separator rather than a comma.";

                    throw new FormatException(text3);
                }
                catch (ArgumentException)
                {
                    throw new FormatException("Attribute '" + text + "' was set more than one time.");
                }
            }

            return attributes;
        }

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, object> item in (IEnumerable<KeyValuePair<string, object>>)this)
            {
                Type type = item.Value.GetType();
                if (type == typeof(string))
                    stringBuilder.AppendLine(item.Key + " \"" + item.Value?.ToString() + "\"");
                else if (type == typeof(int))
                    stringBuilder.AppendLine(item.Key + " " + item.Value);
                else if (type == typeof(double))
                    stringBuilder.AppendLine(item.Key + " " + ((double)item.Value).ToString(".0#######"));
                else
                    stringBuilder.AppendLine(item.Key + " \"" + item.Value.ToString() + "\"");
            }

            return stringBuilder.ToString();
        }
    }

}
