using System;
using Squirrel.Data;

namespace Squirrel.Packets
{
    [Serializable]
    public class PositionPacket : Packet
    {
        public Orientation Orientation { get; set; }

        public PositionPacket()
            : base(PacketType.POSITION_PACKET, -1)
        {
            Orientation = new Orientation();
        }

        public PositionPacket(int clientId, Orientation orientation)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = orientation;
        }

        public PositionPacket(int clientId, float x, float y, float rotation)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
        }

        public override string ToString()
        {
            return base.ToString() + " Orientation: " + Orientation.ToString();
        }
    }
}
