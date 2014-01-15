// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;

namespace Squirrel.Packets
{
    [Serializable]
    public enum PacketType
    {
        EMPTY_PACKET,
        CLIENT_CONNECT_PACKET,
        CLIENT_DISCONNECT_PACKET,
        HEARTBEAT_PACKET,
        CHAT_PACKET,
        POSITION_PACKET,
        PROJECTILE_PACKET
    }
}
