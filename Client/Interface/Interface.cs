using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using Squirrel.Client.Objects;
using Squirrel.Data;

namespace Squirrel.Client.Interface
{
    public partial class Interface : Form
    {
        public static Interface DebugInstance;

        public bool ClientConnected
        {
            get { return m_client != null && m_client.isConnected(); }
        }

        private Client m_client;
        private Thread m_clientThread;
        private readonly Bitmap m_triangle;
        private readonly Stopwatch m_globalTimer = new Stopwatch();

        public Interface()
        {
            DebugInstance = this;
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

        public void updateDebugPanel(float a, float b, float c)
        {
            Invoke((MethodInvoker)delegate
            {
                Text = "" + a + "    " + b + "    " + c;
            });
        }

        public long getTime()
        {
            return m_globalTimer.ElapsedMilliseconds;
        }

        public bool connect(IPAddress ip, int port, string name)
        {
            // Leave if we're already connected
            if (m_client != null && m_client.isConnected())
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

        public void clientChat(string name, string message)
        {
            // Run on the UI thread
            Invoke((MethodInvoker) (() => 
                ChatIncomingTextBox.AppendText("[" + name + "]: " + message + Environment.NewLine)));
        }

        public void onMoved(float delta)
        {
            Player player = (Player) m_client.ClientLocations[m_client.ClientId];

            float dx = delta * player.ForwardSpeed * (float)Math.Cos((player.Orientation.Rotation - 90) * Math.PI / 180.0f);
            float dy = delta * player.ForwardSpeed * (float)Math.Sin((player.Orientation.Rotation - 90) * Math.PI / 180.0f);

            player.move(dx, dy);

            m_client.sendPositionUpdate(player.Orientation);
        }

        public void onRotate(float delta)
        {
            Player player = (Player)m_client.ClientLocations[m_client.ClientId];
            player.rotate(delta * player.RotationSpeed);

            m_client.sendPositionUpdate(player.Orientation);
        }

        public void onClientConnected(Entity entity)
        {
            lock (GameWindow.RenderList)
            {
                // All entities share the same asset right now
                entity.Asset = m_triangle;
                GameWindow.RenderList.Add(entity);
            }
        }

        public void postClientConnectedToChat(int clientId)
        {
            // Run on the UI thread
            Invoke((MethodInvoker)(() =>
                ChatIncomingTextBox.AppendText("Client " + clientId + " connected." + Environment.NewLine)));
        }

        public void onClientDisconnected(Entity entity)
        {
            lock (GameWindow.RenderList)
            {
                GameWindow.RenderList.Remove(entity);
            }
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
                }

                MenuButtonDisconnect.Enabled = false;
                ChatIncomingTextBox.Text = "";
            });
        }

        public void onGameTick()
        {
            m_client.interpolate();
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
