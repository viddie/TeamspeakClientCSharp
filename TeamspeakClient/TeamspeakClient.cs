using PrimS.Telnet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TeamspeakClient.Events;
using TeamspeakClient.Exceptions;
using TeamspeakClient.Helpers;

namespace TeamspeakClient {
    public class TeamspeakClient {

        #region Connection Stuff
        public Client TelClient { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public CancellationToken CancelToken { get; set; }
        #endregion

        private bool ListenerActive { get; set; }

        private Dictionary<Type, List<Action<Event>>> EventCallbacks { get; set; } = new Dictionary<Type, List<Action<Event>>>();
        private Queue<Action<string>> CommandCallbacks { get; set; } = new Queue<Action<string>>();

        public TeamspeakClient() { }
        public TeamspeakClient(string host, int port, CancellationToken ctk) {
            Host = host;
            Port = port;
            CancelToken = ctk;
        }

        public void ConnectClient() {
            TelClient = new Client(Host, Port, CancelToken);
            if (!TelClient.IsConnected) {
                throw new TeamspeakConnectionException("Could not open Telnet connection to client");
            }

            ListenerActive = true;
            new Thread(new ThreadStart(ListenToClient)).Start();
        }

        public void AuthorizeClient(string apiKey) {
            SendCommand($"auth apikey={apiKey}");
        }


        public void SendCommand(string cmd, Action<string> callback=null) {
            string withCallback = callback == null ? "no callback" : "with callback";
            Console.WriteLine($"> '{cmd}' ({withCallback})");

            if (callback == null) {
                callback = (s) => { Console.WriteLine($"Received full message: {s}"); };
            }

            CommandCallbacks.Enqueue(callback);
            TelClient.WriteLine(cmd);
        }

        public string SendCommandSync(string cmd) {
            Console.WriteLine($"> '{cmd}'");
            IsPerformingCommand = true;
            TelClient.WriteLine(cmd);
            lock (Response) {
                Monitor.Wait(Response);
            }
            if (CommandException != null) {
                Exception temp = CommandException;
                CommandException = null;
                throw temp;
            }

            string toRet = Response;
            Response = "";
            return toRet;
        }


        public void RegisterEventCallback(Type eventType, Action<Event> callback) {
            if (!EventCallbacks.ContainsKey(eventType)) {
                string eventToRegister = eventType.Name;
                eventToRegister = eventToRegister.Substring(0, eventToRegister.Length - "Event".Length).ToLower();
                SendCommand($"clientnotifyregister schandlerid=1 event={eventToRegister}");
                EventCallbacks.Add(eventType, new List<Action<Event>>());
            }
            EventCallbacks[eventType].Add(callback);
        }

        public void UnregisterAllCallbacks() {
            SendCommand("clientnotifyunregister");
        }





        #region Communication Listening
        private Exception CommandException { get; set; }
        private string Response { get; set; } = "";
        private bool IsPerformingCommand { get; set; }

        public void ListenToClient() {
            while (ListenerActive) {
                string answer = TelClient.ReadAsync().Result;
                if (string.IsNullOrEmpty(answer)) continue;

                string enc = "Windows-1252";
                //string enc = "iso-8859-1";

                //Dictionary<string, string> encResults = new Dictionary<string, string>();
                //foreach (EncodingInfo info in Encoding.GetEncodings()) {
                //    string encName = info.Name;
                //    Encoding enc = info.GetEncoding();
                //    string asUTF8 = Encoding.UTF8.GetString(enc.GetBytes(answer));
                //    if (!encResults.ContainsKey(encName))
                //        encResults.Add(encName, asUTF8);
                //}

                answer = Encoding.UTF8.GetString(Encoding.GetEncoding(enc).GetBytes(answer));

                string[] lines = answer.Split(new string[] { "\n\r" }, StringSplitOptions.None);
                foreach (string line in lines) ParseInputLine(line.Trim());
            }
        }

        public void ParseInputLine(string line) {
            Console.WriteLine($"< '{line}'");

            if (line.StartsWith("notify") || line.StartsWith("channel")) {
                ReceivedEvcent(line);
            }

            if (IsPerformingCommand) {
                if (line.StartsWith("error")) {
                    if (line.EndsWith("ok")) {
                        IsPerformingCommand = false;
                        lock (Response) {
                            Monitor.PulseAll(Response);
                        }
                    } else {
                        IsPerformingCommand = false;
                        CommandException = new Exception();
                        lock (Response) {
                            Monitor.PulseAll(Response);
                        }
                    }

                } else {
                    Response += $"{line}\n\r";
                }
            }
        }

        public void ReceivedEvcent(string line) {
            string lineList = null;
            string[] attributesSpaceSplit = line.Split(new char[] { ' ' });
            string eventName = attributesSpaceSplit[0];
            Event evt = Event.CreateEventByName(eventName);
            Type type = evt.GetType();
            if (!EventCallbacks.ContainsKey(type)) return;
            if (EventCallbacks[type].Count == 0) return;

            //Event has a return list
            if (evt.ListDividerAttribute != null) {
                int listStartIndex = line.IndexOf(evt.ListDividerAttribute);
                lineList = line.Substring(listStartIndex);
                line = line.Substring(0, listStartIndex).Trim();
            }

            ModelParser.ParseModelFromAttributeString(line, evt, 1);
            List<Dictionary<string, string>> listEntries = null;

            if (lineList != null) {
                string[] listEntriesSplit = lineList.Split(new char[] { '|' });
                listEntries = new List<Dictionary<string, string>>();
                foreach (string listEntry in listEntriesSplit) {
                    evt.AddListItem(listEntry);
                }
            }

            if (EventCallbacks.ContainsKey(type)) {
                foreach (Action<Event> evtCallback in EventCallbacks[type]) {
                    evtCallback.Invoke(evt);
                }
            }
        }



        #endregion


        #region Synchronious Calls
        public List<Client> GetClientList() {
            string response = SendCommandSync("clientlist");
            string[] responseSplit = response.Split(new char[] { '|' });
            List<Client> clients = new List<Client>();
            foreach (string entry in responseSplit) {
                clients.Add(ModelParser.ParseModelFromAttributeString<Client>(entry));
            }

            return clients;
        }
        #endregion







        public static void Main(string[] args) {
            TeamspeakClient client = new TeamspeakClient("localhost", 25639, new CancellationToken());
            client.ConnectClient();
            client.AuthorizeClient("CNP7-MPR4-DXYT-92SE-GEXT-3N2F"); //auth apikey=CNP7-MPR4-DXYT-92SE-GEXT-3N2F
            //client.SendCommand("clientnotifyregister schandlerid=1 event=notifytalkstatuschange");

            List<Client> clients = client.GetClientList();

            RegisterTestEvent(client, typeof(NotifyTalkStatusChangeEvent));
            RegisterTestEvent(client, typeof(NotifyBanListEvent));
            RegisterTestEvent(client, typeof(NotifyMessageEvent));



            //client.RegisterEventCallback(typeof(NotifyTalkStatusChangeEvent), (genericEvt) => {
            //    NotifyTalkStatusChangeEvent evt = (NotifyTalkStatusChangeEvent)genericEvt;
            //    Console.WriteLine($"Got event: {evt}");
            //});


            Console.ReadKey();
        }

        public static void RegisterTestEvent(TeamspeakClient client, Type eventType) {
            client.RegisterEventCallback(eventType, (genericEvt) => {
                Console.WriteLine($"Got event: {genericEvt}");
            });
        }
    }
}
