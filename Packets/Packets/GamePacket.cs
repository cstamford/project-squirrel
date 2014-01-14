using System;
using Squirrel.Data;

namespace Squirrel.Packets
{
    [Serializable]
    public class GamePacket : Packet
    {
        public Orientation Orientation { get; set; }
        public Orientation LastOrientation { get; set; }
        public Vec2F Velocity { get; set; }

        public GamePacket()
            : base(PacketType.POSITION_PACKET, -1)
        {
        }

        public GamePacket(int clientId, Orientation orientation, Orientation lastOrientation, Vec2F velocity)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = orientation;
            LastOrientation = lastOrientation;
            Velocity = velocity;
        }

        public GamePacket(int clientId, float x, float y, float rotation, float lx, float ly, float lrotation, float velx, float vely)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
            LastOrientation = new Orientation(lx, ly, lrotation);
            Velocity = new Vec2F(velx, vely);
        }

        public override string ToString()
        {
            return base.ToString() + " Orientation: " + Orientation.ToString() + " Last Orientation: " +
                   LastOrientation.ToString() + " Velocity: " + Velocity.ToString();
        }
    }
}
