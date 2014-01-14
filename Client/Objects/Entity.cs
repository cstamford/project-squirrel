using System.Drawing;
using Squirrel.Data;

namespace Squirrel.Client.Objects
{
    public class Entity
    {
        public Bitmap Asset { get; set; }
        public Orientation Orientation { get; set; }

        public Entity(Bitmap asset)
        {
            Asset = asset;
            Orientation = new Orientation(0.0f, 0.0f, 0.0f);
        }

        public Entity(Bitmap asset, Orientation orientation)
        {
            Asset = asset;
            Orientation = orientation;
        }

        public Entity(Bitmap asset, float x, float y, float rotation)
        {
            Asset = asset;
            Orientation = new Orientation(x, y, rotation);
        }

        // Move the entity by delta x and delta y
        public void move(float dx, float dy)
        {
            Orientation.Position.x += dx;
            Orientation.Position.y += dy;
        }

        // Rotate by delta
        public void rotate(float delta)
        {
            Orientation.Rotation += delta;
        }
    }
}
