using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Models {

    //cid=2 pid=0 channel_order=0 channel_name=[*spacer8]═ channel_flag_are_subscribed=1 total_clients=0|cid=3 pid=2 channel_order=0 channel_name=Die\sGlocken\släuten channel_flag_are_subscribed=1 total_clients=0

    
    public class Channel : TeamspeakModel {

        [Model("cid")]
        public int Id { get; set; }

        [Model("pid")]
        public int ParentChannelId { get; set; }

        [Model("channel_order")]
        public int ChannelOrder { get; set; }

        [Model("channel_name")]
        public string Name { get; set; }

        [Model("total_clients")]
        public int TotalClients { get; set; }

        #region Flags
        //channel_flag_default=0 channel_flag_password=0 channel_flag_permanent=1 channel_flag_semi_permanent=0 channel_flag_are_subscribed=1 total_clients=0

        [Model("channel_flag_are_subscribed")]
        public bool IsSubscribed { get; set; }

        [Model("channel_flag_default")]
        public bool IsDefault { get; set; }

        [Model("channel_flag_password")]
        public bool HasPassword { get; set; }
        
        [Model("channel_flag_permanent")]
        public bool IsPermanent { get; set; }

        [Model("channel_flag_semi_permanent")]
        public bool IsSemiPermanent { get; set; }

        #endregion


        #region Limits
        //channel_maxclients=-1 channel_maxfamilyclients=-1

        [Model("channel_maxclients")]
        public int MaxClients { get; set; }

        [Model("channel_maxfamilyclients")]
        public bool MaxFamilyClients { get; set; }

        #endregion


        #region Topic
        //channel_topic

        [Model("channel_topic")]
        public string Topic { get; set; }

        #endregion


        #region Voice
        //channel_codec=4 channel_codec_quality=6 channel_needed_talk_power=0

        [Model("channel_codec")]
        public int Codec { get; set; }

        [Model("channel_codec_quality")]
        public int CodecQuality { get; set; }

        [Model("channel_needed_talk_power")]
        public int NeededTalkPower { get; set; }

        #endregion
    }
}
