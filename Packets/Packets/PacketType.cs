using System;

namespace Squirrel.Packets
{
    [Serializable]
    public enum PacketType
    {
        NEW_CLIENT_PACKET,
        HEARTBEAT_PACKET,
        CHAT_PACKET,
        PROJECTILE_PACKET,
        POSITION_PACKET
    }
}
