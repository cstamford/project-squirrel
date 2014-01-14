﻿using System;

namespace Squirrel.Packets
{
    [Serializable]
    public class ClientDisconnectPacket : Packet
    {
        public ClientDisconnectPacket(int clientId)
            : base(PacketType.CLIENT_DISCONNECT_PACKET, clientId)
        {
        }
    }
}
