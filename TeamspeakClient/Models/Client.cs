using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TSClient.Enums;

namespace TSClient.Models {
    public class Client : TeamspeakModel {

        //clid=4872 cid=23 client_database_id=11 client_nickname=eɴĸαмα client_type=0

        [Model("clid")]
        public int Id { get; set; }

        [Model("cid")]
        public int ChannelId { get; set; }

        [Model("client_database_id")]
        public int DatabaseId { get; set; }

        [Model("client_nickname")]
        public string Nickname { get; set; }

        [Model("client_type")]
        public ClientType Type { get; set; }
    }
}
