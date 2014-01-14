using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Squirrel.Client.Objects;
using Squirrel.Data;

namespace Squirrel.Client.Interface
{
    public class GameWindow : UserControl
    {
        // The list of assets to drraw
        public List<Entity> RenderList { get; set; }

        // Mouse location variables
        private readonly Vec2F m_mousePosition = new Vec2F();

        // Mouse clicked flags
        private bool m_mouseLeftPressed;
        private bool m_mouseMiddlePressed;
        private bool m_mouseRightPressed;

        public GameWindow()
        {
            // Set up double buffering and make this control selectable
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.Selectable, true);

            RenderList = new List<Entity>();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.SmoothingMode = SmoothingMode.None;

            // Lock the draw list so nobody can mess with it
            lock (RenderList)
            {
                // Draw each entity
                foreach (Entity ent in RenderList)
                {
                    // Rotate around the centre of the entity
                    e.Graphics.TranslateTransform(ent.Asset.Width / 2.0f, ent.Asset.Height / 2.0f);
                    e.Graphics.RotateTransform(ent.Orientation.Rotation);
                    e.Graphics.TranslateTransform(-ent.Asset.Width / 2.0f, -ent.Asset.Height / 2.0f);

                    // Translate to the actual coordinates
                    // Append this so we rotate first before translating
                    e.Graphics.TranslateTransform(ent.Orientation.Position.x, ent.Orientation.Position.y, MatrixOrder.Append);

                    // Draw the image
                    e.Graphics.DrawImage(ent.Asset, new Point(0, 0));

                    // Reset the transformation matrix for the new entity
                    e.Graphics.ResetTransform();
                }
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            m_mousePosition.x = e.Location.X;
            m_mousePosition.y = e.Location.Y;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            Focus();

            switch (e.Button)
            {
                case MouseButtons.Left:

                    m_mouseLeftPressed = true;
                    break;

                case MouseButtons.Right:

                    m_mouseRightPressed = true;
                    break;

                case MouseButtons.Middle:

                    m_mouseMiddlePressed = true;
                    break;
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:

                    m_mouseLeftPressed = false;
                    break;

                case MouseButtons.Right:

                    m_mouseRightPressed = false;
                    break;

                case MouseButtons.Middle:

                    m_mouseMiddlePressed = false;
                    break;
            }
        }
    }
}