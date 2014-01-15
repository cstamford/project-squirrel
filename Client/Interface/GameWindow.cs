// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Timers;
using System.Windows.Forms;
using Squirrel.Client.Objects;

namespace Squirrel.Client.Interface
{
    public class GameWindow : UserControl
    {
        // The list of assets to drraw
        public List<Entity> RenderList { get; private set; }

        // Key flags
        private bool m_wDown;
        private bool m_aDown;
        private bool m_sDown;
        private bool m_dDown;
        private bool m_spaceDown;

        // Game timer
        private readonly System.Timers.Timer m_forceRefreshTimer = new System.Timers.Timer(Globals.GAME_UPDATES_TICK_TIME);

        public GameWindow()
        {
            // Set up double buffering and make this control selectable
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.Selectable, true);

            RenderList = new List<Entity>();
            m_forceRefreshTimer.Elapsed += timer;
            m_forceRefreshTimer.Enabled = true;
            m_forceRefreshTimer.AutoReset = true;
        }

        void timer(object sender, ElapsedEventArgs e)
        {
            Interface parent = (Interface)ParentForm;

            if (parent == null)
                return;

            // Run on UI Thread
            Invoke((MethodInvoker)Refresh);

            // If we're not connected, there's no point being here
            if (!parent.ClientConnected)
                return;

            parent.onGameTick();

            if (m_wDown)
                parent.onMoved(1.0f);

            if (m_sDown)
                parent.onMoved(-1.0f);

            if (m_aDown)
                parent.onRotate(-1.0f);

            if (m_dDown)
                parent.onRotate(1.0f);

            if (m_spaceDown)
                parent.onProjectile();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.Clear(SystemColors.ControlDark);

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

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                m_wDown = true;

            if (e.KeyCode == Keys.S)
                m_sDown = true;

            if (e.KeyCode == Keys.A)
                m_aDown = true;

            if (e.KeyCode == Keys.D)
                m_dDown = true;

            if (e.KeyCode == Keys.Space)
                m_spaceDown = true;
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.W)
                m_wDown = false;

            if (e.KeyCode == Keys.S)
                m_sDown = false;

            if (e.KeyCode == Keys.A)
                m_aDown = false;

            if (e.KeyCode == Keys.D)
                m_dDown = false;

            if (e.KeyCode == Keys.Space)
                m_spaceDown = false;
        }
    }
}