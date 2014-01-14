﻿using System;
using Squirrel.Data;

namespace Squirrel.Packets
{
    [Serializable]
    public class GamePacket : Packet
    {
        public Orientation Orientation { get; set; }
        public Vec2F Velocity { get; set; }

        public GamePacket()
            : base(PacketType.POSITION_PACKET, -1)
        {
            Orientation = new Orientation();
            Velocity = new Vec2F();
        }

        public GamePacket(int clientId, Orientation orientation, Vec2F velocity)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = orientation;
            Velocity = velocity;
        }

        public GamePacket(int clientId, float x, float y, float rotation, float velx, float vely)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
            Velocity = new Vec2F(velx, vely);
        }

        public override string ToString()
        {
            return base.ToString() + " Orientation: " + Orientation.ToString() + " Velocity: " + Velocity.ToString();
        }
    }
}
