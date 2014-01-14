using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Squirrel.Packets
{
    [Serializable]
    public class ClientDisconnectPacket : Packet
    {
        public ClientDisconnectPacket()
            : base(PacketType.HEARTBEAT_PACKET, -1)
        {
        }

        public ClientDisconnectPacket(int clientId)
            : base(PacketType.HEARTBEAT_PACKET, clientId)
        {
        }
    }
}
