using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakClient.Events {
    public class ChannelListFinishedEvent : Event {

        #region Event Properties

        #endregion

        public override void AddListItem(string itemString) {
            //if (Items == null) Items = new List<Channel>();
            //Channel c = ModelParser.ParseModelFromAttributeString<Channel>(itemString);
            //Items.Add(c);
        }
    }
}
