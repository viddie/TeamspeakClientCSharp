using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakClient.Models {
    public class ModelAttribute :  Attribute {
        public string AttributeName { get; set; }
        public string TimeStampType { get; set; }

        public ModelAttribute(string name) {
            AttributeName = name;
        }
    }
}
