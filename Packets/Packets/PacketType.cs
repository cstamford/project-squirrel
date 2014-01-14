using System;

namespace Squirrel.Packets
{
    [Serializable]
    public enum PacketType
    {
        NEW_CLIENT_PACKET,
        CHAT_PACKET,
        GAME_PACKET
    }
}
