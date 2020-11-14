using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Events {
    public class NotifyChannelDeletedEvent : Event {

        #region Event Properties

        #endregion

        public override void AddListItem(string itemString) {
            //if (Items == null) Items = new List<BanEntry>();
            //BanEntry item = ModelParser.ParseModelFromAttributeString<BanEntry>(itemString);
            //Items.Add(item);
        }
    }
}
