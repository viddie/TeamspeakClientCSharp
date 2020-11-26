using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSClient.Enums;
using TSClient.Models;

namespace TSClient.Events {
    public class NotifyTextMessageEvent : Event {

        #region Event Properties
        //notifytextmessage schandlerid=1 targetmode=1 msg=Hola target=10756 invokerid=10756 invokername=viddie invokeruid=y5UrsvbLV5y9W5nVINyAYYZiU8w=

        [Model("schandlerid")]
        public int ServerConnectionHandlerId { get; set; }

        [Model("targetmode")]
        public MessageMode TargetMode { get; set; }

        [Model("msg")]
        public string Message { get; set; }

        [Model("target")]
        public int Target { get; set; }

        [Model("invokerid")]
        public int InvokerId { get; set; }

        [Model("invokername")]
        public string InvokerName { get; set; }

        [Model("invokeruid")]
        public string InvokerUniqueId { get; set; }

        #endregion

        public override void AddListItem(string itemString) {
            //if (Items == null) Items = new List<BanEntry>();
            //BanEntry item = ModelParser.ParseModelFromAttributeString<BanEntry>(itemString);
            //Items.Add(item);
        }
    }
}
