using System;

namespace Squirrel.Packets
{    
    [Serializable]
    public class NewClientPacket : Packet
    {
        public NewClientPacket()
        {
            PacketType = PacketType.NEW_CLIENT_PACKET;
        }

        public NewClientPacket(int clientId) 
            : base(PacketType.NEW_CLIENT_PACKET, clientId)
        {

        }
    }
}
