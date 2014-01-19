// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System;

namespace Squirrel.Data
{
    [Serializable]
    public class Vec2F
    {
        public float x { get; set; }
        public float y { get; set; }

        public Vec2F()
        {
            x = 0.0f;
            y = 0.0f;
        }

        public Vec2F(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public override string ToString()
        {
            return "X: " + x + " Y: " + y;
        }
    }
}
