using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TSClient.Models;

namespace TSClient.Helpers {
    public class ModelParser {
        public static TModel ParseModelFromAttributeString<TModel>(string line, TModel instance = default, int startingIndex = 0) {
            Console.WriteLine($"Parsing model from attribute string '{line}'");
            PropertyInfo[] properties;
            TModel defaultObj = default;


            //If the object is the default of the class, create a new instance
            if ((instance == null && defaultObj == null) || instance.Equals(defaultObj)) {
                instance = Activator.CreateInstance<TModel>();
                properties = typeof(TModel).GetProperties();
            } else {
                properties = instance.GetType().GetProperties();
            }


            Dictionary<string, string> modelAttributes = ParseAttributeList(line, startingIndex);

            foreach (PropertyInfo property in properties) {
                ModelAttribute attrib = property.GetCustomAttribute<ModelAttribute>();
                if (attrib == null) continue;
                if (!modelAttributes.ContainsKey(attrib.AttributeName)) continue;

                string value = modelAttributes[attrib.AttributeName];
                property.SetValue(instance, ParsePropertyValue<object>(property.PropertyType, value));

                //if (property.PropertyType == typeof(string)) {
                //    property.SetValue(instance, ParseStringAttribute(value));

                //} else if (property.PropertyType == typeof(int)) {
                //    property.SetValue(instance, int.Parse(value));

                //} else if (property.PropertyType == typeof(bool)) {
                //    property.SetValue(instance, value == "1");

                //} else if (property.PropertyType == typeof(DateTime)) {
                //    property.SetValue(instance, DateHelper.UnixTimeStampToDateTime(long.Parse(value)));

                //} else if (property.PropertyType.IsEnum) {
                //    property.SetValue(instance, int.Parse(value));

                //}
            }

            return instance;
        }


        public static T ParsePropertyValue<T>(Type type, string valueStr) {
            object value = null;

            if (type == typeof(string)) {
                value =  ParseStringAttribute(valueStr);

            } else if (type == typeof(int)) {
                value = int.Parse(valueStr);

            } else if (type == typeof(bool)) {
                value = valueStr == "1";

            } else if (type == typeof(DateTime)) {
                value = DateHelper.UnixTimeStampToDateTime(long.Parse(valueStr));

            } else if (type.IsEnum) {
                value = int.Parse(valueStr);
            }

            return (T) value;
        }


        public static T1 ParseAttributeTuple<T1>(string v1) {
            return ParsePropertyValue<T1>(typeof(T1), v1);
        }
        public static (T1, T2) ParseAttributeTuple<T1, T2>(string v1, string v2) {
            return (ParsePropertyValue<T1>(typeof(T1), v1),
                ParsePropertyValue<T2>(typeof(T2), v2));
        }
        public static (T1, T2, T3) ParseAttributeTuple<T1, T2, T3>(string v1, string v2, string v3) {
            return (ParsePropertyValue<T1>(typeof(T1), v1),
                ParsePropertyValue<T2>(typeof(T2), v2),
                ParsePropertyValue<T3>(typeof(T3), v3));
        }
        public static (T1, T2, T3, T4) ParseAttributeTuple<T1, T2, T3, T4>(string v1, string v2, string v3, string v4) {
            return (ParsePropertyValue<T1>(typeof(T1), v1),
                ParsePropertyValue<T2>(typeof(T2), v2),
                ParsePropertyValue<T3>(typeof(T3), v3),
                ParsePropertyValue<T4>(typeof(T4), v4));
        }

        public static Dictionary<string, string> ParseAttributeList(string line, int startIndex = 0) {
            string[] split = line.Split(new char[] { ' ' });
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            for (int i = startIndex; i < split.Length; i++) {
                string parameterPair = split[i];
                string[] parameterPairSplit = parameterPair.Split(new char[] { '=' });
                string key = parameterPairSplit[0];
                string value = null;
                if (parameterPairSplit.Length > 1) value = string.Join("=", parameterPairSplit.Skip(1));
                parameters.Add(key, value);
            }

            return parameters;
        }


        public static string ParseStringAttribute(string value) {
            if (value == null)
                return null;

            value = value.Replace("\\s", " ");
            value = value.Replace("\\p", "|");
            value = value.Replace("\\n", "\n");
            value = value.Replace("\\/", "/");
            value = value.Replace("\\t", "\t");
            return value;
        }

        public static string ToStringAttribute(string value) {
            if (value == null)
                return null;

            value = value.Replace(" ", "\\s");
            value = value.Replace("|", "\\p");
            value = value.Replace("\n", "\\n");
            value = value.Replace("/", "\\/");
            value = value.Replace("\t", "\\t");
            return value;
        }
    }
}
