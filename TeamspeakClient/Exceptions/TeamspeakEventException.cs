using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakClient.Exceptions {
    public class TeamspeakEventException : Exception {
        public TeamspeakEventException() { }
        public TeamspeakEventException(string message) : base(message) { }
        public TeamspeakEventException(string message, Exception innerEx) : base(message, innerEx) { }
    }
}
