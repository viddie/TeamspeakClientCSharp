using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSClient.Enums;
using TSClient.Models;

namespace TSClient.Events {
    public class NotifyClientMovedEvent : Event {

        #region Event Properties
        //SelfMove:     notifyclientmoved schandlerid=1 ctid=21 reasonid=0 clid=4829
        //AdminMove:    notifyclientmoved schandlerid=1 ctid=3 reasonid=1 invokerid=4722 invokername=viddie invokeruid=y5UrsvbLV5y9W5nVINyAYYZiU8w= clid=4828
        //Channel Kick: notifyclientmoved schandlerid=1 ctid=4 reasonid=4 invokerid=4722 invokername=viddie invokeruid=y5UrsvbLV5y9W5nVINyAYYZiU8w= reasonmsg clid=4828

        [Model("schandlerid")]
        public int ServerConnectionHandlerId { get; set; }
        public int ToChannelId { get; set; }
        public ClientMoveReason Reason { get; set; }
        public int InvokerId { get; set; }
        public string InvokerName { get; set; }
        public string InvokerUid { get; set; }
        public string ReasonMassage { get; set; }
        public int ClientId { get; set; }
        #endregion

        //public override void ParseParameters(Dictionary<string, string> parameters, List<Dictionary<string, string>> listEntries = null) {
        //    ServerConnectionHandlerId = int.Parse(parameters["schandlerid"]);
        //    ToChannelId = int.Parse(parameters["ctid"]);
        //    Reason = (ClientMoveReason) int.Parse(parameters["reasonid"]);
        //    ClientId = int.Parse(parameters["clid"]);

        //    if (Reason == ClientMoveReason.AdminMove || Reason == ClientMoveReason.KickedFromChannel) {
        //        InvokerId = int.Parse(parameters["invokerid"]);
        //        InvokerName = parameters["invokername"];
        //        InvokerUid = parameters["invokeruid"];
        //        if(Reason == ClientMoveReason.KickedFromChannel) ReasonMassage = parameters["reasonmsg"];
        //    }
        //}

        public override void AddListItem(string itemString) {
            //if (Items == null) Items = new List<BanEntry>();
            //BanEntry item = ModelParser.ParseModelFromAttributeString<BanEntry>(itemString);
            //Items.Add(item);
        }
    }
}
