// Project Squirrel 
// Copyright 2013-2014 Chris Stamford

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
        public bool ClientConnected
        {
            get { return m_client != null && m_client.isConnected(); }
        }

        private Client m_client;
        private Thread m_clientThread;
        private readonly Bitmap m_triangle;
        private readonly Bitmap m_bubble;
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

            try
            {
                using (Stream BitmapStream = System.IO.File.Open(Path.Combine(assetPath, "bubble.png"),
                        System.IO.FileMode.Open))
                {
                    Image img = Image.FromStream(BitmapStream);
                    m_bubble = new Bitmap(img);
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        // Returns the time in MS
        public long getTime()
        {
            return m_globalTimer.ElapsedMilliseconds;
        }

        // Attempts to connect to the server
        public bool connect(IPAddress ip, int port, string name)
        {
            // Leave if we're already connected
            if (ClientConnected)
                return false;

            // Make a new client to handle this connection attempt
            m_client = new Client(this);

            if (!m_client.connect(ip, port, name)) 
                return false;

            // Start the client thread
            m_clientThread = new Thread(m_client.run);
            m_clientThread.Start();

            MenuButtonDisconnect.Enabled = true;
            SendChatButton.Enabled = true;

            return true;
        }

        // Adds the received chat packet to the incoming textbox
        public void onChatReceived(string name, string message)
        {
            // Run on the UI thread
            Invoke((MethodInvoker) (() => 
                ChatIncomingTextBox.AppendText("[" + name + "]: " + message + Environment.NewLine)));
        }

        // On local user input
        public void onMoved(float delta)
        {
            Player player = (Player) m_client.ClientLocations[m_client.ClientId];

            float dx = delta * player.ForwardSpeed * (float)Math.Cos((player.Orientation.Rotation - 90) * Math.PI / 180.0f);
            float dy = delta * player.ForwardSpeed * (float)Math.Sin((player.Orientation.Rotation - 90) * Math.PI / 180.0f);

            player.move(dx, dy);

            m_client.sendPositionUpdate(player.Orientation);
        }

        // On local user input
        public void onRotate(float delta)
        {
            Player player = (Player)m_client.ClientLocations[m_client.ClientId];
            player.rotate(delta * player.RotationSpeed);

            m_client.sendPositionUpdate(player.Orientation);
        }

        public void onProjectile()
        {
            Player player = (Player)m_client.ClientLocations[m_client.ClientId];
            m_client.createProjectile(player.Orientation);
        }

        // Called when a client has connected
        public void onClientConnected(Entity entity)
        {
            lock (GameWindow.RenderList)
            {
                // All entities share the same asset right now
                entity.Asset = m_triangle;
                GameWindow.RenderList.Add(entity);
            }
        }

        // Adds a projectile
        public void addProjectile(Data.Orientation projectile)
        {
            Projectile proj = new Projectile(m_bubble, projectile);

            lock (m_client.ProjectileList)
            {
                m_client.ProjectileList.Add(proj);
            }

            lock (GameWindow.RenderList)
            {
                GameWindow.RenderList.Add(proj);
            }
        }

        // Add the client join message to the chat box
        public void postClientConnectedToChat(int clientId)
        {
            // Run on the UI thread
            Invoke((MethodInvoker)(() =>
                ChatIncomingTextBox.AppendText("Client " + clientId + " connected." + Environment.NewLine)));
        }

        // Called when a client has disconnected
        public void onClientDisconnected(Entity entity)
        {
            lock (GameWindow.RenderList)
            {
                GameWindow.RenderList.Remove(entity);
            }
        }

        // Add the client leave message to the chatbox
        public void postClientDisconnectedToChat(int clientId)
        {
            // Run on the UI thread
            Invoke((MethodInvoker)(() =>
                ChatIncomingTextBox.AppendText("Client " + clientId + " disconnected." + Environment.NewLine)));
        }

        // Called when disconnected from the server
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
                SendChatButton.Enabled = false;
                ChatIncomingTextBox.Text = "";
                BottomInfoLabel.Text = "DISCONNECTED";
            });
        }

        // Called once every GAME_UPDATE_TICK_TIME ms
        public void onGameTick()
        {
            // Scan for projectiles that need to be removed
            lock (m_client.ProjectileList)
            {
                for (int i = 0; i < m_client.ProjectileList.Count; ++i)
                {
                    Projectile projectile = m_client.ProjectileList[i];

                    lock (GameWindow.RenderList)
                    {
                        for (int j = 0; j < GameWindow.RenderList.Count; ++j)
                        {
                            Entity ent = GameWindow.RenderList[j];

                            if (ent is Projectile && ent == projectile && 
                                (projectile.Orientation.Position.x < -50 ||
                                projectile.Orientation.Position.y < -50 ||
                                projectile.Orientation.Position.x > Width ||
                                projectile.Orientation.Position.y > Height ))
                            {
                                GameWindow.RenderList.Remove(ent);
                                m_client.ProjectileList.Remove(projectile);
                            }
                        }
                    }
                }
            }

            m_client.interpolate();
            updateBottomBar();
        }

        // Updates the bottom bar with appropriate informati
        private void updateBottomBar()
        {
            int numClients = m_client.ClientLocations.Count;
            Player player = (Player)m_client.ClientLocations[m_client.ClientId];

            // Run on the UI thread
            Invoke((MethodInvoker)delegate
            {
                BottomInfoLabel.Text = "CONNECTED" +
                    "                Client ID: " + m_client.ClientId + 
                    "                Clients: " + numClients +
                    "                Location:" + player.Orientation.ToString();

                BottomInfoLabel.Invalidate();
            });
        }

        private void Interface_Load(object sender, System.EventArgs e)
        {
            m_globalTimer.Start();
            GameWindow.Focus();
        }

        private void Interface_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (ClientConnected)
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

        private void ChatOutgoingTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                SendChatButton.PerformClick();
            }
        }
    }
}
