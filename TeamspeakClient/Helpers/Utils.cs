using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSClient.Models;

namespace TSClient.Helpers {
    public static class Utils {

        public static void PrintClientList(List<Client> clients, Action<string> printTarget=null) {
            if (printTarget == null) {
                printTarget = Console.WriteLine;
            }

            printTarget.Invoke("Client List:");
            foreach (Client client in clients) {
                printTarget.Invoke($"- {client}");
            }
        }
    }
}
