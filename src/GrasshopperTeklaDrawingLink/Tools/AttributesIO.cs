using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Tekla.Structures.Drawing;

namespace GTDrawingLink.Tools
{
    public class AttributesIO
    {
        public enum AttributeTypeEnum
        {
            auto,
            @string,
            @int,
            @float,
            date
        }

        public static readonly DateTime DATETIME_EPOCH = new DateTime(1970, 1, 1);

        public static List<Attributes> GetAll(DatabaseObject modelObject)
        {
            var attributes = new List<Attributes>();

            Dictionary<string, string> stringDict;
            Dictionary<string, int> intDict;
            Dictionary<string, double> dblDict;

            if (modelObject is Plugin plugin)
            {
                stringDict = typeof(Plugin).GetField("LocalStringValues", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(plugin) as Dictionary<string, string>;

                if (stringDict.Any())
                {
                    var attribute = Tools.Attributes.Parse(stringDict);
                    attributes.Add(attribute);
                }

                intDict = typeof(Plugin).GetField("LocalIntValues", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(plugin) as Dictionary<string, int>;

                if (intDict.Any())
                {
                    var attribute = Tools.Attributes.Parse(intDict);
                    attributes.Add(attribute);
                }

                dblDict = typeof(Plugin).GetField("LocalDoubleValues", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(plugin) as Dictionary<string, double>;

                if (dblDict.Any())
                {
                    var attribute = Tools.Attributes.Parse(dblDict);
                    if (attribute.Count > 0)
                        attributes.Add(attribute);
                }
            }
            else
            {
                if (modelObject.GetStringUserProperties(out stringDict) && stringDict.Any())
                    attributes.Add(Tools.Attributes.Parse(stringDict));

                if (modelObject.GetIntegerUserProperties(out intDict) && intDict.Any())
                    attributes.Add(Tools.Attributes.Parse(intDict));

                if (modelObject.GetDoubleUserProperties(out dblDict) && dblDict.Any())
                    attributes.Add(Tools.Attributes.Parse(dblDict));
            }

            return attributes;
        }

        public static void SetUDAs(DatabaseObject databaseObject, Attributes uDAs)
        {
            if (uDAs == null)
                return;

            foreach (KeyValuePair<string, object> UDA in uDAs)
            {
                if (UDA.Value is double value)
                    databaseObject.SetUserProperty(UDA.Key, value);
                else if (UDA.Value is int value2)
                    databaseObject.SetUserProperty(UDA.Key, value2);
                else if (UDA.Value is string value3)
                    databaseObject.SetUserProperty(UDA.Key, value3);
            }
        }
        public static object GetUDAValue(DatabaseObject obj, string udaName, AttributeTypeEnum type)
        {
            switch (type)
            {
                case AttributeTypeEnum.auto:
                    {
                        var asInt = GetUDAValue(obj, udaName, AttributeTypeEnum.@int);
                        if (asInt != null)
                            return asInt;

                        var asDouble = GetUDAValue(obj, udaName, AttributeTypeEnum.@float);
                        if (asDouble != null)
                            return asDouble;

                        var asString = GetUDAValue(obj, udaName, AttributeTypeEnum.@string);
                        if (asString != null)
                            return asString;

                        break;
                    }
                case AttributeTypeEnum.@float:
                    {
                        var doubleValue = 0.0;
                        if (obj.GetUserProperty(udaName, ref doubleValue))
                            return doubleValue;

                        break;
                    }
                case AttributeTypeEnum.@int:
                    {
                        var intValue = 0;
                        if (obj.GetUserProperty(udaName, ref intValue))
                            return intValue;

                        break;
                    }
                case AttributeTypeEnum.date:
                    {
                        var intValue = 0;
                        if (obj.GetUserProperty(udaName, ref intValue))
                            return DATETIME_EPOCH.AddSeconds(intValue);

                        break;
                    }
                default:
                    {
                        string strValue = null;
                        if (obj.GetUserProperty(udaName, ref strValue))
                            return strValue;

                        break;
                    }
            }

            return null;
        }

        public static object GetComponentAttributeValue(Plugin plugin, string name, AttributeTypeEnum type)
        {
            switch (type)
            {
                case AttributeTypeEnum.auto:
                    {
                        var asInt = GetComponentAttributeValue(plugin, name, AttributeTypeEnum.@int);
                        if (asInt != null)
                            return asInt;

                        var asDouble = GetComponentAttributeValue(plugin, name, AttributeTypeEnum.@float);
                        if (asDouble != null)
                            return asDouble;

                        var asString = GetComponentAttributeValue(plugin, name, AttributeTypeEnum.@string);
                        if (asString != null)
                            return asString;

                        break;
                    }
                case AttributeTypeEnum.@float:
                    {
                        double doubleValue = 0.0;
                        if (plugin.TryGetAttribute(name, ref doubleValue))
                            return doubleValue;

                        break;
                    }
                case AttributeTypeEnum.@int:
                    {
                        int intValue = 0;
                        if (plugin.TryGetAttribute(name, ref intValue))
                            return intValue;

                        break;
                    }
                case AttributeTypeEnum.date:
                    {
                        int intValue = 0;
                        if (plugin.TryGetAttribute(name, ref intValue))
                            return DATETIME_EPOCH.AddSeconds(intValue);

                        break;
                    }
                default:
                    {
                        string strValue = null;
                        if (plugin.TryGetAttribute(name, ref strValue))
                            return strValue;

                        break;
                    }
            }

            return null;
        }
    }
}
