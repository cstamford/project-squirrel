using System;
using Squirrel.Data;

namespace Squirrel.Packets
{
    [Serializable]
    public class GamePacket : Packet
    {
        public Orientation Orientation { get; set; }
        public Vec2F Vector { get; set; }
        public Vec2F Velocity { get; set; }

        public GamePacket()
            : base(PacketType.POSITION_PACKET, -1)
        {
            Orientation = new Orientation();
            Vector = new Vec2F();
            Velocity = new Vec2F();
        }

        public GamePacket(int clientId, Orientation orientation, Vec2F vector, Vec2F velocity)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = orientation;
            Vector = vector;
            Velocity = velocity;
        }

        public GamePacket(int clientId, float x, float y, float rotation, float vecx, float vecy, float velx, float vely)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
            Vector = new Vec2F(vecx, vecy);
            Velocity = new Vec2F(velx, vely);
        }

        public override string ToString()
        {
            return base.ToString() + " Orientation: " + Orientation.ToString() + " Last Orientation: " +
                   LastOrientation.ToString() + " Velocity: " + Velocity.ToString();
        }
    }
}
