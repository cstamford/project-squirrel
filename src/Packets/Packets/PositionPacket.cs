// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;
using Squirrel.Data;

namespace Squirrel.Packets
{
    [Serializable]
    public class PositionPacket : Packet
    {
        public Orientation Orientation { get; set; }
        public bool OverrideLocal { get; set; }

        public PositionPacket()
            : base(PacketType.POSITION_PACKET, -1)
        {
            Orientation = new Orientation();
            OverrideLocal = false;
        }

        public PositionPacket(int clientId, Orientation orientation, bool owrite = false)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = orientation;
            OverrideLocal = owrite;
        }

        public PositionPacket(int clientId, float x, float y, float rotation, bool owrite = false)
            : base(PacketType.POSITION_PACKET, clientId)
        {
            Orientation = new Orientation(x, y, rotation);
            OverrideLocal = owrite;
        }

        public override string ToString()
        {
            return base.ToString() + " Orientation: " + Orientation.ToString() + " Override: " + OverrideLocal;
        }
    }
}
