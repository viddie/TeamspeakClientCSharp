using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakClient.Models {
    public abstract class TeamspeakModel {

        public override string ToString() {
            string typeName = this.GetType().Name;
            string toRet = $"[ {typeName}, ";

            foreach (PropertyInfo info in this.GetType().GetProperties()) {
                ModelAttribute attrib = info.GetCustomAttribute<ModelAttribute>();
                if (attrib == null) continue;

                string key = info.Name;
                object objValue = info.GetValue(this);
                string value = "null";
                if (objValue != null) value = objValue.ToString();
                toRet += $"{key}='{value}', ";
            }

            toRet = toRet.Substring(0, toRet.Length - 2);
            toRet += " ]";

            return toRet;
        }
    }
}
