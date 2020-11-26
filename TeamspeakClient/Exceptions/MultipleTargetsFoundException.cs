using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSClient.Models;

namespace TSClient.Exceptions {

    [Serializable]
    public class MultipleTargetsFoundException : Exception {

        public List<Client> AllFoundTargets { get; set; }

        public MultipleTargetsFoundException() { }
        public MultipleTargetsFoundException(string message) : base(message) { }
        public MultipleTargetsFoundException(string message, Exception innerEx) : base(message, innerEx) { }
    }
}
