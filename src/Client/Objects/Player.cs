// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System.Drawing;
using Squirrel.Data;

namespace Squirrel.Client.Objects
{
    public class Player : Entity
    {
        public Player(Bitmap asset)
            : base(asset)
        { 
        }

        public Player(Bitmap asset, Orientation orientation)
            : base(asset, orientation, Globals.DEFAULT_SPEED, Globals.DEFAULT_SPEED)
        {
        }

        public Player(Bitmap asset, float x, float y, float rotation)
            : base(asset, x, y, rotation, Globals.DEFAULT_SPEED, Globals.DEFAULT_SPEED)
        {
        }
    }
}
