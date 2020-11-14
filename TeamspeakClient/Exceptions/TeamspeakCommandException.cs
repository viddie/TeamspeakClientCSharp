using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Exceptions {

    [Serializable]
    public class TeamspeakCommandException : Exception {
        public TeamspeakCommandException() { }
        public TeamspeakCommandException(string message) : base(message) { }
        public TeamspeakCommandException(string message, Exception innerEx) : base(message, innerEx) { }
    }
}
