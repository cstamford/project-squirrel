using System.Drawing;
using Squirrel.Data;

namespace Squirrel.Client.Objects
{
    public class Entity
    {
        private Bitmap m_asset;
        private Orientation m_orientation;

        public Entity(Bitmap asset)
        {
            m_asset = asset;
            m_orientation = new Orientation(0.0f, 0.0f, 0.0f);
        }

        public Entity(Bitmap asset, Orientation orientation)
        {
            m_asset = asset;
            m_orientation = orientation;
        }

        public Entity(Bitmap asset, float x, float y, float rotation)
        {
            m_asset = asset;
            m_orientation = new Orientation(x, y, rotation);
        }

        // Get the entity's aboslute x position
        public float getX()
        {
            return m_orientation.Position.x;
        }

        // Get the entity's absolute y position
        public float getY()
        {
            return m_orientation.Position.y;
        }

        // Get the entity's rotation
        public float getRotation()
        {
            return m_orientation.Rotation;
        }

        // Get the entity's asset
        public Bitmap getAsset()
        {
            return m_asset;
        }

        // Set the entity's absolute x position
        public void setX(float x)
        {
            m_orientation.Position.x = x;
        }

        // Set the entity's absolute y position
        public void setY(float y)
        {
            m_orientation.Position.y = y;
        }

        // Set the entity's asset
        public void setAsset(Bitmap asset)
        {
            m_asset = asset;
        }

        // Set the entity's absolute position
        public void setPosition(float x, float y)
        {
            m_orientation.Position.x = x;
            m_orientation.Position.y = y;
        }

        // Set the entity's absolute orientation
        public void setOrientation(Orientation orientation)
        {
            m_orientation = orientation;
        }

        // Set the entity's absolute orientation
        public void setOrientation(float x, float y, float rotation)
        {
            m_orientation.Position.x = x;
            m_orientation.Position.y = y;
            m_orientation.Rotation = rotation;
        }

        // Move the entity by delta x and delta y
        public void move(float dx, float dy)
        {
            m_orientation.Position.x += dx;
            m_orientation.Position.y += dy;
        }

        public void rotation(float delta)
        {
            m_orientation.Rotation += delta;
        }
    }
}
