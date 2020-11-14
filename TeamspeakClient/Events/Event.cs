using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TeamspeakClient.Events {

    public abstract class Event {

        public static readonly Dictionary<string, Type> EventTypesByName = new Dictionary<string, Type>() {
            ["notifytalkstatuschange"] = typeof(NotifyTalkStatusChangeEvent),
            ["notifymessage"] = typeof(NotifyMessageEvent),
            ["notifymessagelist"] = typeof(NotifyMessageListEvent),
            ["notifycomplainlist"] = typeof(NotifyComplainListEvent),
            ["notifybanlist"] = typeof(NotifyBanListEvent),
            ["notifyclientmoved"] = typeof(NotifyClientMovedEvent),
            ["notifyclientleftview"] = typeof(NotifyClientLeftViewEvent),
            ["notifycliententerview"] = typeof(NotifyClientEnterViewEvent),
            ["notifyclientpoke"] = typeof(NotifyClientPokeEvent),
            ["notifyclientchatclosed"] = typeof(NotifyClientChatClosedEvent),
            ["notifyclientchatcomposing"] = typeof(NotifyClientChatComposingEvent),
            ["notifyclientupdated"] = typeof(NotifyClientUpdatedEvent),
            ["notifyclientids"] = typeof(NotifyClientIdsEvent),
            ["notifyclientdbidfromuid"] = typeof(NotifyClientDbidFromUidEvent),
            ["notifyclientnamefromuid"] = typeof(NotifyClientNameFromUidEvent),
            ["notifyclientnamefromdbid"] = typeof(NotifyClientNameFromDbidEvent),
            ["notifyclientuidfromclid"] = typeof(NotifyClientUidFromClidEvent),
            ["notifyconnectioninfo"] = typeof(NotifyConnectionInfoEvent),
            ["notifychannelcreated"] = typeof(NotifyChannelCreatedEvent),
            ["notifychanneledited"] = typeof(NotifyChannelEditedEvent),
            ["notifychanneldeleted"] = typeof(NotifyChannelDeletedEvent),
            ["notifychannelmoved"] = typeof(NotifyChannelMovedEvent),
            ["notifyserveredited"] = typeof(NotifyServerEditedEvent),
            ["notifyserverupdated"] = typeof(NotifyServerUpdatedEvent),
            ["channellist"] = typeof(ChannelListEvent),
            ["channellistfinished"] = typeof(ChannelListFinishedEvent),
            ["notifytextmessage"] = typeof(NotifyTextMessageEvent),
            ["notifycurrentserverconnectionchanged"] = typeof(NotifyCurrentServerConnectionChangedEvent),
            ["notifyconnectstatuschange"] = typeof(NotifyConnectStatusChangeEvent),
        };
        public static Event CreateEventByName(string eventName) {
            return (Event)Activator.CreateInstance(EventTypesByName[eventName]);
        }

        public string ListDividerAttribute { get; set; } = null;

        public abstract void AddListItem(string itemString);


        public override string ToString() {
            string typeName = this.GetType().Name;
            string toRet = $"[ {typeName}, ";
            
            foreach (PropertyInfo info in this.GetType().GetProperties()) {
                string key = info.Name;
                if (key == "Parameters" || key == "EventTypesByName" || key == "Items") continue;
                object objValue = info.GetValue(this);
                string value = "null";
                if (objValue != null) value = objValue.ToString();
                toRet += $"{key}='{value}', ";
            }

            toRet = toRet.Substring(0, toRet.Length - 2);
            toRet += " ]";

            return toRet;
        }
    }
}
