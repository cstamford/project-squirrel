using System.Drawing;
using Squirrel.Data;

namespace Squirrel.Client.Objects
{
    public class Entity
    {
        public Bitmap Asset { get; set; }
        public Orientation Orientation { get; set; }
        public Orientation RemoteOrientation { get; set; }

        public float ForwardSpeed { get; set; }
        public float RotationSpeed { get; set; }

        public Entity(Bitmap asset)
        {
            Asset = asset;
            Orientation = new Orientation(0.0f, 0.0f, 0.0f);
            RemoteOrientation = new Orientation(0.0f, 0.0f, 0.0f);
            ForwardSpeed = Globals.DEFAULT_SPEED;
            RotationSpeed = Globals.DEFAULT_SPEED;
        }

        public Entity(Bitmap asset, Orientation orientation, float forwardSpeed, float rotSpeed)
        {
            Asset = asset;
            Orientation = orientation;
            RemoteOrientation = new Orientation(Orientation.Position.x, Orientation.Position.y, orientation.Rotation);
            ForwardSpeed = forwardSpeed;
            RotationSpeed = rotSpeed;
        }

        public Entity(Bitmap asset, float x, float y, float rotation, float forwardSpeed, float rotSpeed)
        {
            Asset = asset;
            Orientation = new Orientation(x, y, rotation);
            RemoteOrientation = new Orientation(x, y, rotation);
            ForwardSpeed = forwardSpeed;
            RotationSpeed = rotSpeed;
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

            if (Orientation.Rotation > 360.0f)
                Orientation.Rotation -= 360.0f;

            if (Orientation.Rotation < 0.0f)
                Orientation.Rotation += 360.0f;
        }

        public void interpolate(float delta = 1.0f)
        {
            
        }
    }
}
