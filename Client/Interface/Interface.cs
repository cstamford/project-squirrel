using System;
using System.Diagnostics;
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
        private readonly Stopwatch m_globalTimer = new Stopwatch();

        public Interface()
        {
            InitializeComponent();

            string assetPath =
                Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "assets");
            try
            {
                using (Stream BitmapStream = System.IO.File.Open(Path.Combine(assetPath, "triangle.png"),
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

        public long getTime()
        {
            return m_globalTimer.ElapsedMilliseconds;
        }

        private void Interface_Load(object sender, System.EventArgs e)
        {
            m_globalTimer.Start();
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

        public void clientChat(string name, string message)
        {
            // Run on the UI thread
            Invoke((MethodInvoker) (() => 
                ChatIncomingTextBox.AppendText("[" + name + "]: " + message + Environment.NewLine)));
        }

        public void clientMoved(int clientId, Entity entity)
        {
        }

        public void clientConnected(Entity entity)
        {
            // Run on the UI thread
            Invoke((MethodInvoker) delegate
            {
                lock (GameWindow.RenderList)
                {
                    // All entities share the same asset right now
                    entity.Asset = m_triangle;
                    GameWindow.RenderList.Add(entity);
                    GameWindow.Invalidate();
                }
            });
        }

        public void postClientConnectedToChat(int clientId)
        {
            // Run on the UI thread
            Invoke((MethodInvoker)(() =>
                ChatIncomingTextBox.AppendText("Client " + clientId + " connected." + Environment.NewLine)));
        }

        public void clientDisconnected(Entity entity)
        {
            // Run on the UI thread
            Invoke((MethodInvoker) delegate
            {
                lock (GameWindow.RenderList)
                {
                    GameWindow.RenderList.Remove(entity);
                    GameWindow.Invalidate();
                }
            });
        }

        public void postClientDisconnectedToChat(int clientId)
        {
            // Run on the UI thread
            Invoke((MethodInvoker)(() =>
                ChatIncomingTextBox.AppendText("Client " + clientId + " disconnected." + Environment.NewLine)));
        }


        private void SendChatButton_Click(object sender, EventArgs e)
        {
            string text = ChatOutgoingTextBox.Text;
            ChatOutgoingTextBox.Text = "";

            m_client.sendChatMessage(text);
        }
    }
}
