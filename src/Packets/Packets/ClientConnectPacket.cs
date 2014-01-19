// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;
using Squirrel.Data;

namespace Squirrel.Packets
{    
    [Serializable]
    public class ClientConnectPacket : Packet
    {
        public Orientation Orientation { get; set; }

        public ClientConnectPacket()
            : base(PacketType.CLIENT_CONNECT_PACKET, -1)
        {
            Orientation = new Orientation();
        }

        public ClientConnectPacket(int clientId, Orientation orientation) 
            : base(PacketType.CLIENT_CONNECT_PACKET, clientId)
        {
            Orientation = orientation;
        }

        public ClientConnectPacket(int clientId, float x, float y, float rotation)
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
