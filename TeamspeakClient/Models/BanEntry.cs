using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Models {
    public class BanEntry : TeamspeakModel {
        //banid=104 ip name uid=4rFCcLZ6OzyUpQs1HXBcyweoVAI= mytsid lastnickname=Claus\sLäufer\sUmlaut created=1602977645 duration=0 invokername=philip invokercldbid=7 invokeruid=fX+eZK05tZ+emzm+NhPzugtFMbo= reason enforcements=0
        [Model("banid")]
        public int BandId { get; set; }

        [Model("ip")]
        public string IpAddress { get; set; }

        [Model("name")]
        public string Name { get; set; }

        [Model("uid")]
        public string UniqueId { get; set; }

        [Model("mytsid")]
        public string MyTeamspeakId { get; set; }

        [Model("lastnickname")]
        public string LastNickname { get; set; }

        [Model("created")]
        public DateTime Created { get; set; }

        [Model("duration")]
        public DateTime Duration { get; set; }

        [Model("invokername")]
        public string InvokerName { get; set; }

        [Model("invokercldbid")]
        public int InvokerClientDbId { get; set; }

        [Model("invokeruid")]
        public string InvokerUid { get; set; }

        [Model("reason")]
        public string Reason { get; set; }

        [Model("enforcements")]
        public int Enforcements { get; set; }
    }
}
