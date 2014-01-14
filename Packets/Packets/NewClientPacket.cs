using System;
using Squirrel.Data;

namespace Squirrel.Packets
{    
    [Serializable]
    public class NewClientPacket : Packet
    {
        public Orientation Orientation { get; set; }

        public NewClientPacket()
        {
            PacketType = PacketType.NEW_CLIENT_PACKET;
        }

        public NewClientPacket(int clientId, Orientation orientation) 
            : base(PacketType.NEW_CLIENT_PACKET, clientId)
        {
            Orientation = orientation;
        }

        public NewClientPacket(int clientId, float x, float y, float rotation)
            : base(PacketType.NEW_CLIENT_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
        }

        public override string ToString()
        {
            return base.ToString() + " " + Orientation.ToString();
        }
    }
}
