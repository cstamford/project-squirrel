using System;

namespace Squirrel.Data
{
    [Serializable]
    public class Orientation
    {
        public Vec2F Position { get; set; }
        public float Rotation { get; set; }

        public Orientation()
        {
            Position = new Vec2F();
            Rotation = 0.0f;
        }

        public Orientation(Vec2F position, float rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public Orientation(float x, float y, float rotation)
        {
            Position = new Vec2F(x, y);
            Rotation = rotation;
        }

        public override string ToString()
        {
            return Position.ToString() + " Rotation: " + Rotation;
        }
    }
}
