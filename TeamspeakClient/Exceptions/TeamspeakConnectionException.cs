using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Exceptions {
    [Serializable]
    public class TeamspeakConnectionException : Exception {
        public TeamspeakConnectionException() { }
        public TeamspeakConnectionException(string message) : base(message) { }
        public TeamspeakConnectionException(string message, Exception innerEx) : base(message, innerEx) { }
    }
}
