using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Exceptions {
    [Serializable]
    public class NoTargetsFoundException : Exception {
        public NoTargetsFoundException() { }
        public NoTargetsFoundException(string message) : base(message) { }
        public NoTargetsFoundException(string message, Exception innerEx) : base(message, innerEx) { }
    }
}
