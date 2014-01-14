using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using System.Windows.Forms;
using Squirrel.Client.Objects;

namespace Squirrel.Client.Interface
{
    public partial class Interface : Form
    {
        private readonly List<Entity> m_objects = new List<Entity>();
        private Client m_client;

        public Interface()
        {
            InitializeComponent();
            GameWindow.SetDrawList(m_objects);
        }

        private void Interface_Load(object sender, System.EventArgs e)
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

                    m_objects.Add(new Entity(bitmap, 150.0f, 0.0f, 45.0f));
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }

            try
            {
                m_client = new Client();
                MessageBox.Show(m_client.connect(IPAddress.Parse("127.0.0.1"), 37500) ? "Connected" : "Failed to connect");
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }
    }
}
