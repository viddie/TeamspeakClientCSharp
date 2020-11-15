using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TSClient.Events;
using TSClient.Exceptions;
using TSClient.Helpers;
using TSClient.Models;

namespace TSClient {
    public class TeamspeakClient {

        #region Connection Stuff
        public PrimS.Telnet.Client TelClient { get; set; }
        public string Host { get; set; }
        public int Port { get; set; }
        public CancellationToken CancelToken { get; set; }
        #endregion

        private bool ListenerActive { get; set; }

        private Dictionary<Type, List<Action<Event>>> EventCallbacks { get; set; } = new Dictionary<Type, List<Action<Event>>>();
        private Queue<Action<Exception, string>> CommandCallbacks { get; set; } = new Queue<Action<Exception, string>>();

        #region Cached Properties
        private List<Client> AllClients;
        #endregion



        public TeamspeakClient(string host, int port, CancellationToken ctk = new CancellationToken()) {
            Host = host;
            Port = port;
            CancelToken = ctk;
        }

        public void ConnectClient() {
            TelClient = new PrimS.Telnet.Client(Host, Port, CancelToken);
            if (!TelClient.IsConnected) {
                throw new TeamspeakConnectionException("Could not open Telnet connection to client");
            }

            ListenerActive = true;
            new Thread(new ThreadStart(ListenToClient)).Start();
        }

        public void CloseClient() {
            SendCommand("quit");
        }
        public void CleanupClient() {
            TelClient.Dispose();
        }

        public void AuthorizeClient(string apiKey) {
            SendCommand($"auth apikey={apiKey}");
        }


        public void SendCommand(string cmd, Action<Exception, string> callback=null) {
            string withCallback = callback == null ? "no callback" : "with callback";
            Console.WriteLine($"> '{cmd}' ({withCallback})");

            if (callback == null) {
                callback = (ex, s) => GenericCallback(cmd, ex, s);
            }

            CommandCallbacks.Enqueue(callback);
            TelClient.WriteLine(cmd);
        }

        static int i = 0;
        public string SendCommandSync(string cmd) {
            object isDone = $"Command: {cmd} ({i++})";
            Exception exception = null;
            string response = null;


            SendCommand(cmd, (ex, r) => {
                Console.WriteLine($"Got response for synchronious message '{isDone}': {r}");
                exception = ex;
                response = r;
                lock (isDone) {
                    Monitor.Pulse(isDone);
                }
            });

            lock (isDone) {
                Monitor.Wait(isDone);
            }

            if (exception != null) {
                throw new TeamspeakCommandException($"Encountered an exception during execution of command '{cmd}'", exception);
            }

            return response;
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
                string answer;
                try {
                    answer = TelClient.ReadAsync().Result;
                } catch (Exception) {
                    return;
                }

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

        //notifyclientmoved schandlerid=1 ctid=23 reasonid=0 clid=10481  clid=10481 cid=23 client_database_id=9 client_nickname=viddie client_type=0
        /*
            > 'clientmove cid=9 clid=10641' (no callback)
            > 'clientlist' (with callback)
            < 'clid=10481 cid=23 client_database_id=9 client_nickname=viddie client_type=0|clid=10487 cid=21 client_database_id=7 client_nickname=mave client_type=0|clid=10546 cid=21 client_database_id=582 client_nickname=fly client_type=0|clid=10641 cid=10 client_database_id=834 client_nickname=TeamSpeakUser client_type=0'
            < 'error id=0 msg=ok'
            Received full message for command 'clientmove cid=9 clid=10641': clid=10481 cid=23 client_database_id=9 client_nickname=viddie client_type=0|clid=10487 cid=21 client_database_id=7 client_nickname=mave client_type=0|clid=10546 cid=21 client_database_id=582 client_nickname=fly client_type=0|clid=10641 cid=10 client_database_id=834 client_nickname=TeamSpeakUser client_type=0
            < ''
            < 'notifyclientmoved schandlerid=1 ctid=9 reasonid=0 clid=10641'
            Parsing model from attribute string 'notifyclientmoved schandlerid=1 ctid=9 reasonid=0 clid=10641'
            < ''
            > 'clientlist' (with callback)
            < 'clid=10481 cid=23 client_database_id=9 client_nickname=viddie client_type=0|clid=10487 cid=21 client_database_id=7 client_nickname=mave client_type=0|clid=10546 cid=21 client_database_id=582 client_nickname=fly client_type=0|clid=10641 cid=9 client_database_id=834 client_nickname=TeamSpeakUser client_type=0'
            < 'error id=0 msg=ok'
            Got response for synchronious message 'Command: clientlist (2)': clid=10481 cid=23 client_database_id=9 client_nickname=viddie client_type=0|clid=10487 cid=21 client_database_id=7 client_nickname=mave client_type=0|clid=10546 cid=21 client_database_id=582 client_nickname=fly client_type=0|clid=10641 cid=9 client_database_id=834 client_nickname=TeamSpeakUser client_type=0
            < ''
            Parsing model from attribute string 'clid=10481 cid=23 client_database_id=9 client_nickname=viddie client_type=0'
            Parsing model from attribute string 'clid=10487 cid=21 client_database_id=7 client_nickname=mave client_type=0'
            Parsing model from attribute string 'clid=10546 cid=21 client_database_id=582 client_nickname=fly client_type=0'
            Parsing model from attribute string 'clid=10641 cid=9 client_database_id=834 client_nickname=TeamSpeakUser client_type=0'
            < 'error id=0 msg=ok'
            Got response for synchronious message 'Command: clientlist (3)': 
             */

        public void ParseInputLine(string line) {
            Console.WriteLine($"< '{line}'");

            line = line.Trim();

            if (line.Contains("\n\r") || Response.Contains("\n\r")) {
                int i = 0;
            }

            if (line.StartsWith("notify") || line.StartsWith("channel")) {
                ReceivedEvcent(line);
                return;
            }

            if (CommandCallbacks.Count > 0) {
                if (line.StartsWith("error")) {
                    Action<Exception, string> callback = CommandCallbacks.Dequeue();
                    Response = Response.Trim();

                    if (line.EndsWith("ok")) {
                        IsPerformingCommand = false;
                        callback.Invoke(null, Response);

                    } else {
                        IsPerformingCommand = false;
                        Console.WriteLine($"Command did not execute successfully (last line '{line}'). Leftover response: {Response}");
                        CommandException = new TeamspeakCommandException($"Command did not execute successfully (last line '{line}'). Leftover response: {Response}");
                        callback.Invoke(CommandException, Response);
                    }

                    Response = "";

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
                    Task.Run(() => evtCallback.Invoke(evt));
                }
            }
        }



        #endregion


        #region Synchronious Calls

        #region Client List Calls
        public List<Client> GetClientList(bool fromCache=true) {
            if (fromCache && AllClients != null) {
                return AllClients;
            }

            string response = SendCommandSync("clientlist");

            string[] responseSplit = response.Split(new char[] { '|' });
            AllClients = new List<Client>();
            foreach (string entry in responseSplit) {
                AllClients.Add(ModelParser.ParseModelFromAttributeString<Client>(entry));
            }

            return AllClients;
        }

        public List<Client> GetClientsInChannel(int channelId, bool fromCache=true) {
            List<Client> clientList = GetClientList(fromCache);
            List<Client> clientsInChannel = new List<Client>();

            foreach (Client client in clientList) {
                if (client.ChannelId == channelId) {
                    clientsInChannel.Add(client);
                }
            }

            return clientsInChannel;
        }


        public List<Client> GetUnmutedClients(List<Client> baseList=null) {
            if (baseList == null)
                baseList = GetClientList();

            List<Client> toRet = new List<Client>();

            foreach (Client client in baseList) {
                if (IsClientInputMuted(client) || IsClientOutputMuted(client)) {
                    toRet.Add(client);
                }
            }

            return toRet;
        }

        #endregion

        #region Client Calls
        public bool IsClientInputMuted(Client client) {
            Dictionary<string, string> result = ModelParser.ParseAttributeList(SendCommandSync($"clientvariable clid={client.Id} client_input_muted"));
            return ModelParser.ParseAttributeTuple<bool>(result["client_input_muted"]);
        }

        public bool IsClientOutputMuted(Client client) {
            Dictionary<string, string> result = ModelParser.ParseAttributeList(SendCommandSync($"clientvariable clid={client.Id} client_output_muted"));
            return ModelParser.ParseAttributeTuple<bool>(result["client_output_muted"]);
        }
        #endregion

        public (int, int) GetWhoAmI() {
            Dictionary<string, string> resultWhoami = ModelParser.ParseAttributeList(SendCommandSync("whoami"));
            return ModelParser.ParseAttributeTuple<int, int>(resultWhoami["clid"], resultWhoami["cid"]);
        }
        #endregion






        /*
        */
        public static void Main(string[] args) {
            TeamspeakClient client = new TeamspeakClient("localhost", 25639, new CancellationToken());
            client.ConnectClient();
            client.AuthorizeClient("CNP7-MPR4-DXYT-92SE-GEXT-3N2F");

            //List<Client> clients = client.GetClientList();

            RegisterTestEvent(client, typeof(NotifyTalkStatusChangeEvent));
            RegisterTestEvent(client, typeof(NotifyBanListEvent));
            RegisterTestEvent(client, typeof(NotifyMessageEvent));
            RegisterTestEvent(client, typeof(NotifyClientMovedEvent));

            (int clientId, int channelId) = client.GetWhoAmI();

            Console.WriteLine($"     ===> My Client ID:  {clientId}");
            Console.WriteLine($"     ===> My Channel ID: {channelId}");

            List<Client> inMyChannel = client.GetClientsInChannel(channelId);
            Utils.PrintClientList(inMyChannel);

            Console.WriteLine($"All Clients:");
            List<Client> clients = client.GetClientList(false);
            Utils.PrintClientList(clients);

            //Task.Run(() => {
            //    while (true) {
            //        Console.WriteLine("Command 'whoami': "+client.SendCommandSync("whoami"));
            //        Thread.Sleep(4000);
            //    }
            //});
            //Thread.Sleep(0);
            //Task.Run(() => {
            //    while (true) {
            //        Console.WriteLine("Command 'clientlist': "+client.SendCommandSync("clientlist"));
            //        Thread.Sleep(4000);
            //    }
            //});


            Console.ReadKey();
        }


        public static void RegisterTestEvent(TeamspeakClient client, Type eventType) {
            client.RegisterEventCallback(eventType, (genericEvt) => {
                Console.WriteLine($"Got event: {genericEvt}");
            });
        }

        public static void GenericCallback(string cmd, Exception ex, string response) {
            if (ex == null) {
                Console.WriteLine($"Received full message for command '{cmd}': {response}");
            } else {
                Console.WriteLine($"Received exception for command '{cmd}': {ex}\nResponse: {response}");
            }
        }
    }
}







// ============ COMMAND LIST ================
/*

auth apikey=CNP7-MPR4-DXYT-92SE-GEXT-3N2F

Command Overview:
   help                        | read help files
   quit                        | close connection
   use                         | select server connection handler
        use [schandlerid={scHandlerID}] [{scHandlerID}]

   auth                        | authenticate telnet connection with users API key
        auth apikey={string}




   banadd                      | add a new ban rule to the server
         banadd [ip={regexp}] [name={regexp}] [uid={clientUID}] [time={timeInSeconds}] [banreason={text}]

   banclient                   | ban a client from the server
        banclient clid={clientID}|cldbid={clientDatabaseID}|uid={clientUID} [time={timeInSeconds}] [banreason={text}]

   bandelall                   | delete all active ban rules
        bandelall

   bandel                      | delete an active ban rule from the server
        bandel banid={banID}

   banlist                     | list all active ban rules
        banlist

   channeladdperm              | add a permission to a channel
        channeladdperm cid={channelID} ( permid={permID}|permsid={permName} permvalue={permValue} )...

   channelclientaddperm        | add a channel-client permisison to a client and specified channel id


   channelclientdelperm        | delete a channel-client permisison from a client and specified channel id


   channelclientlist           | displays a list of clients that are in the channel specified by the cid parameter


   channelclientpermlist       | list all assigned channel-client permisisons for a client and specified channel id


   channelconnectinfo          | channel connect information (path '[cspacer4]╠\sLaberecken\s╣\/Esselam\s#4'
        channelconnectinfo [cid={channelid}]

   channelcreate               | create a channel


   channeldelete               | delete a channel


   channeldelperm              | delete a from a channel


   channeledit                 | edit a channel


   channelgroupadd             | create a channel group


   channelgroupaddperm         | add a permission to a channel group


   channelgroupclientlist      | list all assigned channel groups for the specified channel id


   channelgroupdel             | delete a channel group


   channelgroupdelperm         | delete a permission from a channel group


   channelgrouplist            | list all available channel groups


   channelgrouppermlist        | list all assigned permissions from a channel group


   channellist                 | list of all channels


   channelmove                 | assign a new parent channel to a channel


   channelpermlist             | list all assigned permissions for a channel


   channelvariable             | retrieve specific information about a channel


   clientaddperm               | add a permission to a clientDBID


   clientdbdelete              | delete a client from the server database


   clientdbedit                | edit a clients properties identified by clientDBID


   clientdblist                | list all clients stored in the server database


   clientdelperm               | delete a permission from a clientDBID


   clientgetdbidfromuid        | get the clientDBIDs for a certain client unique id


   clientgetids                | get the clientIDs for a certain client unique id


   clientgetnamefromdbid       | get the nickname from a client database id


   clientgetnamefromuid        | get the nickname from a client unique id


   clientgetuidfromclid        | get the unique id from a clientID


   clientkick                  | kick a client
        clientkick reasonid={4|5} [reasonmsg={text}] clid={clientID}...          Example: clientkick reasonid=4 reasonmsg=Go\saway! clid=5|clid=6

   clientlist                  | list known clients


   clientmove                  | move a client or switch channel ourself
        clientmove cid={channelID} [cpw={channelPassword}] clid={clientID}...    Example: clientmove cid=3 clid=5|clid=6

   clientmute                  | mute all voice data from a client
        clientmute clid={clientID1}...

   clientunmute                | unmute a previously muted client
        clientunmute clid={clientID1}...

   clientnotifyregister        | register to receive client notifications


   clientnotifyunregister      | unregister from receiving client notifications


   clientpermlist              | list all assigned permissions from a clientDBID


   clientpoke                  | poke a client
        clientpoke msg={txt} clid={clientID}

   clientupdate                | set personal client variables, like your nickname
        clientupdate ident=value...

        Sets one or more values concerning your own client, and makes them available
        to other clients through the server where applicable. Available idents are:

        client_nickname:             set a new nickname
        client_away:                 0 or 1, set us away or back available
        client_away_message:         what away message to display when away
        client_input_muted:          0 or 1, mutes or unmutes microphone
        client_output_muted:         0 or 1, mutes or unmutes speakers/headphones
        client_input_deactivated:    0 or 1, same as input_muted, but invisible to other clients
        client_is_channel_commander: 0 or 1, sets or removes channel commander
        client_nickname_phonetic:    set your phonetic nickname
        client_flag_avatar:          set your avatar
        client_meta_data:            any string that is passed to all clients that have vision of you.
        client_default_token:        privilege key to be used for the next server connect

   clientvariable              | retrieve specific information about a client
        clientvariable ( clid={clientID} properties )...

        Retrieves client variables from the client (no network usage). For each client
        you can specify one or more properties that should be queried, and this whole
        block of clientID and properties can be repeated to get information about
        multiple clients with one call of clientvariable.

        Available properties are:
        client_unique_identifier
        client_nickname
        client_input_muted
        client_output_muted
        client_outputonly_muted
        client_input_hardware
        client_output_hardware
        client_meta_data
        client_is_recording
        client_database_id
        client_channel_group_id
        client_servergroups
        client_away
        client_away_message
        client_type
        client_flag_avatar
        client_talk_power
        client_talk_request
        client_talk_request_msg
        client_description
        client_is_talker
        client_is_priority_speaker
        client_unread_messages
        client_nickname_phonetic
        client_needed_serverquery_view_power
        client_icon_id
        client_is_channel_commander
        client_country
        client_channel_group_inherited_channel_id
        client_flag_talking
        client_is_muted
        client_volume_modificator

        These properties are always available for yourself, but need to be requested
        for other clients. Currently you cannot request these variables via
        clientquery:
        client_version
        client_platform
        client_login_name
        client_created
        client_lastconnected
        client_totalconnections
        client_month_bytes_uploaded
        client_month_bytes_downloaded
        client_total_bytes_uploaded
        client_total_bytes_downloaded

        These properties are available only for yourself:
        client_input_deactivated

   complainadd                 | submit a complaint about a clientDBID


   complaindelall              | delete all complaints from a clientDBID


   complaindel                 | delete a complaint from the server


   complainlist                | list all complaints from a server or for a clientDBID


   connect                     | connects to a server in current server tab


   currentschandlerid          | server connection handler ID of current server tab


   disconnect                  | disconnects from server in current server tab


   ftcreatedir                 | create a new directory


   ftdeletefile                | delete one or more files


   ftgetfileinfo               | get informations about the specified file


   ftgetfilelist               | list all files for the specified channel and filepath


   ftinitdownload              | initialise a filetransfer download


   ftinitupload                | initialise a filetransfer upload


   ftlist                      | get a list of all file transfers currently running on the server  notifyfiletransferlist


   ftrenamefile                | rename the specified file


   ftstop                      | stop an running file transfer progress


   hashpassword                | create a password hash


   messageadd                  | send an offline message to a clientDBID


   messagedel                  | delete an existing offline message from your inbox


   messageget                  | display an existing offline message from your inbox


   messagelist                 | list all offline messages from your inbox


   messageupdateflag           | mark or unmark an offline message as read


   permoverview                | list all assigned permissons


   sendtextmessage             | send a chat message


   serverconnectinfo           | server connect information


   serverconnectionhandlerlist | list available server connection handlers


   servergroupaddclient        | add a client to a server group


   servergroupadd              | create a server group


   servergroupaddperm          | add a permission to a server group


   servergroupclientlist       | list all client database ids from a server group


   servergroupdelclient        | delete a client from a server group


   servergroupdel              | delete a server group


   servergroupdelperm          | delete a permission from a server group


   servergrouplist             | get a list of server groups


   servergrouppermlist         | list all assigned permission from a server group


   servergroupsbyclientid      | get all assigned server groups from a clientDBID


   servervariable              | retrieve specific information about a server


   setclientchannelgroup       | assign a channel group to a client database id


   tokenadd                    | add a token to a server- or channel group


   tokendelete                 | delete an existing token from the server


   tokenlist                   | lists all tokens available on the server


   tokenuse                    | use a token to gain access to the server


   verifychannelpassword       | check if we know the current password of a channel


   verifyserverpassword        | check if we know the current server password


   whoami                      | display information about ourself


*/




