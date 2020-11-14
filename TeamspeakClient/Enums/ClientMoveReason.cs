using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSClient.Enums {
    public enum ClientMoveReason {
        SelfMove = 0,
        AdminMove = 1,
        KickedFromChannel = 4,
    }
}
