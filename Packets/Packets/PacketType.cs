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
        PROJECTILE_PACKET,
        POSITION_PACKET
    }
}
