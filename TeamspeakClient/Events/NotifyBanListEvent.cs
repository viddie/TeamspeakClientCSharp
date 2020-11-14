using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSClient.Helpers;
using TSClient.Models;

namespace TSClient.Events {
    public class NotifyBanListEvent : Event {

        #region Event Properties
        //notifybanlist schandlerid=1 count=4

        [Model("schandlerid")]
        public int ServerConnectionHandlerId { get; set; }

        [Model("count")]
        public int Count { get; set; }
        #endregion

        public NotifyBanListEvent() {
            ListDividerAttribute = "banid";
        }

        public List<BanEntry> Items { get; set; }

        public override void AddListItem(string itemString) {
            if (Items == null) Items = new List<BanEntry>();
            BanEntry item = ModelParser.ParseModelFromAttributeString<BanEntry>(itemString);
            Items.Add(item);
        }
    }
}
