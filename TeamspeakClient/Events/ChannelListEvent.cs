using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeamspeakClient.Helpers;
using TeamspeakClient.Models;

namespace TeamspeakClient.Events {
    public class ChannelListEvent : Event {

        #region Event Properties

        #endregion

        public List<Channel> Items { get; set; }

        public override void AddListItem(string itemString) {
            //if (Items == null) Items = new List<Channel>();
            //Channel c = ModelParser.ParseModelFromAttributeString<Channel>(itemString);
            //Items.Add(c);
        }
    }
}
