using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamspeakClient.Enums;
using TeamspeakClient.Models;

namespace TeamspeakClient.Events {
    public class NotifyTalkStatusChangeEvent : Event {

        #region Event Properties
        //schandlerid='1', status='0', isreceivedwhisper='0', clid='4722'
        [Model("schandlerid")]
        public int ServerConnectionHandlerId { get; set; }

        [Model("status")]
        public TalkStatus Status { get; set; }

        [Model("isreceivedwhisper")]
        public bool IsReceivedWhisper { get; set; }

        [Model("clid")]
        public int ClientId { get; set; }
        #endregion

        //public override void ParseParameters(Dictionary<string, string> parameters, List<Dictionary<string, string>> listEntries = null) {
        //    ServerConnectionHandlerId = int.Parse(parameters["schandlerid"]);
        //    Status = (TalkStatus) int.Parse(parameters["status"]);
        //    IsReceivedWhisper = parameters["isreceivedwhisper"] == "1";
        //    ClientId = int.Parse(parameters["clid"]);
        //}

        public override void AddListItem(string itemString) {
            //if (Items == null) Items = new List<BanEntry>();
            //BanEntry item = ModelParser.ParseModelFromAttributeString<BanEntry>(itemString);
            //Items.Add(item);
        }
    }
}
