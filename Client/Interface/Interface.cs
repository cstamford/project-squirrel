using System;
using System.Collections.Generic;
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
        private static readonly List<Entity> m_objects = new List<Entity>();
        private Client m_client;
        private Thread m_clientThread;

        public Interface()
        {
            InitializeComponent();
            GameWindow.SetDrawList(m_objects);
        }

        private void Interface_Load(object sender, System.EventArgs e)
        {
            try
            {
                m_client = new Client();

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

        public static void addEntity(Entity entity)
        {
            string assetPath =
                Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "assets");
            try
            {
                using (Stream BitmapStream = System.IO.File.Open(Path.Combine(assetPath, "triangle.png"), System.IO.FileMode.Open))
                {
                    Image img = Image.FromStream(BitmapStream);
                    Bitmap bitmap = new Bitmap(img);

                    entity.setAsset(bitmap);
                    m_objects.Add(entity);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
