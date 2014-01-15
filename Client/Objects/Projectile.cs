using System;
using System.Drawing;
using Squirrel.Data;

namespace Squirrel.Client.Objects
{
    public class Projectile : Entity
    {
        public Projectile(Bitmap asset)
            : base(asset)
        { 
        }

        public Projectile(Bitmap asset, Orientation orientation)
            : base(asset, orientation, Globals.DEFAULT_PROJECTILE_SPEED, Globals.DEFAULT_PROJECTILE_SPEED)
        {
        }

        public Projectile(Bitmap asset, float x, float y, float rotation)
            : base(asset, x, y, rotation, Globals.DEFAULT_PROJECTILE_SPEED, Globals.DEFAULT_PROJECTILE_SPEED)
        {
        }

        // Just move the projectiles every frame by their speed
        public override void interpolate(float delta = 1)
        {
            Orientation = RemoteOrientation;

            float dx = delta * ForwardSpeed * (float)Math.Cos((Orientation.Rotation - 90) * Math.PI / 180.0f);
            float dy = delta * ForwardSpeed * (float)Math.Sin((Orientation.Rotation - 90) * Math.PI / 180.0f);

            move(dx, dy);

            RemoteOrientation = Orientation;
        }
    }
}
