using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
