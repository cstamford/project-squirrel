using System;

namespace Squirrel.Packets
{
    [Serializable]
    public class ChatPacket : Packet
    {
        public string Message { get; set; }

        public ChatPacket()
            : base(PacketType.CHAT_PACKET, -1)
        {
        }

        public ChatPacket(int clientId, string message)
            : base(PacketType.CHAT_PACKET, clientId)
        {
            Message = message;
        }

        public override string ToString()
        {
            return base.ToString() + " Message: " + Message;
        }
    }
}
