﻿// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;

namespace Squirrel.Packets
{
    [Serializable]
    public class HeartbeatPacket : Packet
    {
        public HeartbeatPacket()
            : base(PacketType.HEARTBEAT_PACKET, -1)
        {
        }

        public HeartbeatPacket(int clientId)
            : base(PacketType.HEARTBEAT_PACKET, clientId)
        {
        }
    }
}
