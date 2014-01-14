using System;

namespace Squirrel.Packets
{
    [Serializable]
    public class ChatPacket : Packet
    {
        public string Name { get; set; }
        public string Message { get; set; }

        public ChatPacket()
            : base(PacketType.CHAT_PACKET, -1)
        {
        }

        public ChatPacket(int clientId, string name, string message)
            : base(PacketType.CHAT_PACKET, clientId)
        {
            Name = name;
            Message = message;
        }

        public override string ToString()
        {
            return base.ToString() + " Name: " + Name + " Message: " + Message;
        }
    }
}
