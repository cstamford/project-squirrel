using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms.VisualStyles;
using Squirrel.Client.Objects;
using Squirrel.Data;
using Squirrel.Packets;

namespace Squirrel.Client
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public static Dictionary<int, Entity> ClientLocations { get; set; }

        private long m_lastHeartbeat;

        private readonly Stopwatch m_timer = new Stopwatch();
        private readonly Stopwatch m_globalTimer = new Stopwatch();

        private static Connection m_connection;
        private static IPEndPoint m_endPoint;
        private bool m_connected = false;

        public Client()
        {
            ClientLocations = new Dictionary<int, Entity>();
        }

        public bool connect(IPAddress ip, int port, string name)
        {
            if (m_connected)
                return false;

            // Set the client's name (for chat)
            Name = name;

            // Set initial client ID to error value
            ClientId = -1;

            // Generate the endpoint
            m_endPoint = new IPEndPoint(ip, port);

            // Build a new connection
            m_connection = new Connection();

            // Set up the TCP socket
            m_connection.TcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // Allow us to bind UDP and TCP to the same port
            m_connection.TcpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // Block here - UI thread will terminate this thread it takes too long
            m_connection.TcpSocket.Connect(m_endPoint);

            byte[] rawBuffer = new byte[Globals.PACKET_BUFFER_SIZE];

            try
            {
                // Now that we've got a TCP connection, receive client ID and orientation from the server
                m_connection.TcpSocket.Receive(rawBuffer, rawBuffer.Count(), SocketFlags.None);

                // Unbundle the packets received
                Packet[] packetsReceived = Packet.unbundle(rawBuffer);

                // Select the orientation packet from the bundle
                foreach (NewClientPacket ncPacket in packetsReceived.Where(packet => packet.PacketType == PacketType.NEW_CLIENT_PACKET).Cast<NewClientPacket>())
                {
                    ClientId = ncPacket.ClientId;

                    // Use the client ID from the server
                    m_connection.ClientId = ClientId;

                    // Start at the position from the server
                    ClientLocations[m_connection.ClientId] = new Player(null, ncPacket.Orientation);
                    Interface.Interface.addEntity(ClientLocations[m_connection.ClientId]);
                }
            }
            // Failed to receive or unbundle the orientation packet
            // Connection attempt failed
            catch
            {
                closeConnection();
                return false;
            }

            // Client ID is still -1 for some reason
            // Connection attempt failed
            if (ClientId == -1)
            {
                closeConnection();
                return false;
            }

            // Creates a UDP socket to go with our TCP socket
            m_connection.UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            // Allow us to bind UDP and TCP to the same port
            m_connection.UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            // Connect!
            m_connection.UdpSocket.Connect(m_endPoint);

            m_connected = true;

            return true;
        }

        public void run()
        {
            m_timer.Start();
            m_globalTimer.Start();

            while (m_connected)
            {
                if (m_globalTimer.ElapsedMilliseconds <= m_lastHeartbeat + Globals.PACKET_HEARTBEAT_FREQUENCY)
                    continue;

                lock (m_connection)
                {
                    byte[] packet = Packet.bundle(new HeartbeatPacket(ClientId));
                    m_connection.TcpSocket.Send(packet);
                    m_lastHeartbeat = m_globalTimer.ElapsedMilliseconds;
                }
            }
        }

        public void onOrientationChange(Orientation newOrientation)
        {
            
        }

        public void closeConnection()
        {
            lock (m_connection)
            {
                if (m_connection.TcpSocket != null)
                {
                    m_connection.TcpSocket.Close();
                    m_connection.TcpSocket = null;
                }

                if (m_connection.UdpSocket != null)
                {
                    m_connection.UdpSocket.Close();
                    m_connection.UdpSocket = null;
                }

                m_connected = false;
            }
        }

        public bool isConnected()
        {
            return m_connected;
        }
    }
}
