// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;
using Squirrel.Data;

namespace Squirrel.Packets
{
    [Serializable]
    public class ProjectilePacket : Packet
    {
        public Orientation Orientation { get; set; }

        public ProjectilePacket()
            : base(PacketType.PROJECTILE_PACKET, -1)
        {
            Orientation = new Orientation();
        }

        public ProjectilePacket(int clientId, Orientation orientation) 
            : base(PacketType.PROJECTILE_PACKET, clientId)
        {
            Orientation = orientation;
        }

        public ProjectilePacket(int clientId, float x, float y, float rotation)
            : base(PacketType.CLIENT_CONNECT_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
        }

        public override string ToString()
        {
            return base.ToString() + " " + Orientation.ToString();
        }
    }
}
