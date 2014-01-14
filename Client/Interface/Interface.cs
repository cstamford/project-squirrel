using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using Squirrel.Client.Objects;

namespace Squirrel.Client.Interface
{
    public partial class Interface : Form
    {
        private Client m_client;
        private Thread m_clientThread;

        private readonly Bitmap m_triangle;

        public Interface()
        {
            InitializeComponent();

            string assetPath =
                Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "assets");
            try
            {
                using (
                    Stream BitmapStream = System.IO.File.Open(Path.Combine(assetPath, "triangle.png"),
                        System.IO.FileMode.Open))
                {
                    Image img = Image.FromStream(BitmapStream);
                    m_triangle = new Bitmap(img);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void Interface_Load(object sender, System.EventArgs e)
        {
            m_client = new Client(this);

            try
            {
                if (m_client.connect(IPAddress.Parse("127.0.0.1"), 37500, "Test"))
                {
                    m_clientThread = new Thread(m_client.run);
                    m_clientThread.Start();
                }
                else
                {
                    MessageBox.Show("Failed to connect");
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        private void Interface_FormClosing(object sender, FormClosingEventArgs e)
        {
            m_client.closeConnection();

            if (m_clientThread != null)
            {
                m_clientThread.Join();
            }
        }

        public void clientMoved(int clientId, Entity entity)
        {
        }

        public void clientConnected(Entity entity)
        {
            lock (GameWindow.RenderList)
            {
                // All entities share the same asset right now
                entity.Asset = m_triangle;
                GameWindow.RenderList.Add(entity);
            }
        }

        public void clientDisconnected(Entity entity)
        {
            lock (GameWindow.RenderList)
            {
                GameWindow.RenderList.Remove(entity);
            }
        }
    }
}
