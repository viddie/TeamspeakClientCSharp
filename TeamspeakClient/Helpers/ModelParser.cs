using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TeamspeakClient.Models;

namespace TeamspeakClient.Helpers {
    public class ModelParser {
        public static TModel ParseModelFromAttributeString<TModel>(string line, TModel instance = default, int startingIndex = 0) {
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

                string value = modelAttributes[attrib.AttributeName];

                if (property.PropertyType == typeof(string)) {
                    property.SetValue(instance, ParseStringAttribute(value));

                } else if (property.PropertyType == typeof(int)) {
                    property.SetValue(instance, int.Parse(value));

                } else if (property.PropertyType == typeof(bool)) {
                    property.SetValue(instance, value == "1");

                } else if (property.PropertyType == typeof(DateTime)) {
                    property.SetValue(instance, DateHelper.UnixTimeStampToDateTime(long.Parse(value)));

                } else if (property.PropertyType.IsEnum) {
                    property.SetValue(instance, int.Parse(value));

                }
            }

            return instance;
        }


        public static Dictionary<string, string> ParseAttributeList(string line, int startIndex = 0) {
            string[] split = line.Split(new char[] { ' ' });
            Dictionary<string, string> parameters = new Dictionary<string, string>();
            for (int i = startIndex; i < split.Length; i++) {
                string parameterPair = split[i];
                string[] parameterPairSplit = parameterPair.Split(new char[] { '=' });
                string key = parameterPairSplit[0];
                string value = null;
                if (parameterPairSplit.Length > 1) value = parameterPairSplit[1];
                parameters.Add(key, value);
            }

            return parameters;
        }


        public static string ParseStringAttribute(string value) {
            value = value.Replace("\\s", " ");
            value = value.Replace("\\p", "|");
            value = value.Replace("\\n", "\n");
            return value;
        }
    }
}
