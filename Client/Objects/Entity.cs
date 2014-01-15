﻿using System;
using System.ComponentModel;
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
            Orientation.Rotation = wrapRotation(Orientation.Rotation);
        }

        public static float differenceBetween(float angle1, float angle2)
        {
            return 180.0f - Math.Abs(Math.Abs(angle1 - angle2) - 180.0f);
        }

        public static float wrapRotation(float rotation)
        {
            if (rotation > 360.0f)
                rotation -= 360.0f;

            else if (rotation < 0.0f)
                rotation += 360.0f;

            return rotation;
        }

        // Simple interpolation that moves current position towards
        // network position at a speed faster than movement speed - therefore accounting
        // for latency
        public void interpolate(float delta = 1.0f)
        {
            float angle1 = Orientation.Rotation;
            float angle2 = RemoteOrientation.Rotation;

            if (angle1 > angle2)
            {
                Orientation.Rotation -= delta;
            }
            else
            {
                Orientation.Rotation += delta;
            }





            float dx = delta * ForwardSpeed * (float)Math.Cos((Orientation.Rotation - 90) * Math.PI / 180.0f);

            if (dx < 0)
                dx *= -1;

            float dy = delta * ForwardSpeed * (float)Math.Sin((Orientation.Rotation - 90) * Math.PI / 180.0f);

            if (dy < 0)
                dy *= -1;

            
            // The current position is higher than the remote / desired position
            // We need to move down
            if (Orientation.Position.y < RemoteOrientation.Position.y)
            {
                Orientation.Position.y += dy;

                // If we're now below
                if (Orientation.Position.y > RemoteOrientation.Position.y)
                    Orientation.Position.y = RemoteOrientation.Position.y;
            }
            // We need to move up
            else if (Orientation.Position.y > RemoteOrientation.Position.y)
            {
                Orientation.Position.y -= dy;

                // If we're now above
                if (Orientation.Position.y < RemoteOrientation.Position.y)
                    Orientation.Position.y = RemoteOrientation.Position.y;
                
            }


            // The current position is to the left of the remote / desired position
            // We need to move right
            if (Orientation.Position.x < RemoteOrientation.Position.x)
            {
                Orientation.Position.x += dx;

                // If we're now too far right
                if (Orientation.Position.x > RemoteOrientation.Position.x)
                    Orientation.Position.x = RemoteOrientation.Position.x;
            }
            // We need to move left
            else if (Orientation.Position.x > RemoteOrientation.Position.x)
            {
                Orientation.Position.x -= dx;

                // If we're now too far left
                if (Orientation.Position.x < RemoteOrientation.Position.x)
                    Orientation.Position.x = RemoteOrientation.Position.x;
            }
            
        }
    }
}
