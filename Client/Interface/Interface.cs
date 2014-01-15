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

        public bool connect(IPAddress ip, int port, string name)
        {
            // Leave if we're already connected
            if (m_client != null && !m_client.isConnected())
                return false;

            // Make a new client to handle this connection attempt
            m_client = new Client(this);

            if (!m_client.connect(ip, port, name)) 
                return false;

            // Start the client thread
            m_clientThread = new Thread(m_client.run);
            m_clientThread.Start();

            MenuButtonDisconnect.Enabled = true;

            return true;
        }

        private void Interface_Load(object sender, System.EventArgs e)
        {
            m_globalTimer.Start();
        }

        private void Interface_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (m_client != null)
                m_client.closeConnection(false);
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

        public void onDisconnect()
        {           
            // Run on the UI thread
            Invoke((MethodInvoker) delegate
            {
                lock (GameWindow.RenderList)
                {
                    GameWindow.RenderList.Clear();
                    GameWindow.Invalidate();
                }

                MenuButtonDisconnect.Enabled = false;
            });
        }

        private void SendChatButton_Click(object sender, EventArgs e)
        {
            string text = ChatOutgoingTextBox.Text;
            ChatOutgoingTextBox.Text = "";

            m_client.sendChatMessage(text);
        }

        private void MenuButtonConnect_Click(object sender, EventArgs e)
        {
            ConnectDialog connectDialog = new ConnectDialog();
            connectDialog.ShowDialog(this);
        }

        private void MenuButtonDisconnect_Click(object sender, EventArgs e)
        {
            m_client.closeConnection();
        }

        private void MenuButtonExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
