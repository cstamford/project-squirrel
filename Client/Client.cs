using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Squirrel.Client.Objects;
using Squirrel.Data;
using Squirrel.Packets;


namespace Squirrel.Client
{
    public class Client
    {
        public int ClientId { get; set; }
        public string Name { get; set; }
        public Dictionary<int, Entity> ClientLocations { get; set; }

        private long m_lastHeartbeat;

        private readonly Stopwatch m_timer = new Stopwatch();
        private readonly Interface.Interface m_parentInterface;

        private readonly List<Packet> m_tcpPacketQueue = new List<Packet>();
        private readonly List<Packet> m_udpPacketQueue = new List<Packet>(); 

        private Connection m_connection;
        private IPEndPoint m_endPoint;
        private bool m_connected = false;

        public Client(Interface.Interface parentInterface)
        {
            m_parentInterface = parentInterface;
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

            // Set the remote endpoint to the TCP socket's target (the server
            m_connection.RemoteEndPoint = m_connection.TcpSocket.RemoteEndPoint;

            byte[] rawBuffer = new byte[Globals.PACKET_BUFFER_SIZE];

            try
            {
                // Now that we've got a TCP connection, receive client ID and orientation from the server
                m_connection.TcpSocket.Receive(rawBuffer, rawBuffer.Count(), SocketFlags.None);

                // Unbundle the packets received
                Packet[] packetsReceived = Packet.unbundle(rawBuffer);

                // Select the orientation packet from the bundle
                foreach (ClientConnectPacket ncPacket in packetsReceived.Where(packet => packet.PacketType == PacketType.CLIENT_CONNECT_PACKET).Cast<ClientConnectPacket>())
                {
                    ClientId = ncPacket.ClientId;

                    // Use the client ID from the server
                    m_connection.ClientId = ClientId;

                    // Start at the position from the server
                    ClientLocations[m_connection.ClientId] = new Player(null, ncPacket.Orientation);

                    // Send a client connected message to the interface, so it can update
                    m_parentInterface.onClientConnected(ClientLocations[m_connection.ClientId]);
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

            // Bind to the local endpoint
            m_connection.UdpSocket.Bind(m_connection.TcpSocket.LocalEndPoint);

            m_connected = true;

            return true;
        }

        public void run()
        {
            m_timer.Start();

            while (m_connected)
            {
                handleIncomingMessages();

                handleHeartbeat();

                // Process the outbound queues
                if (!(m_timer.ElapsedMilliseconds > Globals.NETWORK_UPDATES_TICK_TIME)) 
                    continue;

                // Handle outgoing messages
                handleOutgoingMessages();

                // Clear the queues
                clearQueue(m_tcpPacketQueue);
                clearQueue(m_udpPacketQueue);

                // Restart the timer
                m_timer.Restart();
            }
        }

        public bool isConnected()
        {
            return m_connected;
        }

        public void sendPositionUpdate(Orientation newOrientation)
        {
            addPacketToQueue(m_udpPacketQueue, new PositionPacket(ClientId, newOrientation));
        }

        public void sendChatMessage(string message)
        {
            addPacketToQueue(m_tcpPacketQueue, new ChatPacket(ClientId, Name, message));
        }

        public void closeConnection(bool closeSockets = true)
        {
            m_connected = false;

            if (!Connection.connectionValid(m_connection) || !closeSockets)
                return;

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
            }

            m_parentInterface.onDisconnect();
        }

        public void interpolate()
        {
            lock (ClientLocations)
            {
                foreach (KeyValuePair<int, Entity> pair in ClientLocations.Where(pair => pair.Key != ClientId))
                {
                    pair.Value.interpolate();
                }
            }
        }

        private void updateClientPosition(int clientId, Orientation orientation)
        {
            lock (ClientLocations)
            {
                // If the client is already here
                if (ClientLocations.ContainsKey(clientId))
                {
                    // Just update the remote orientation
                    ClientLocations[clientId].RemoteOrientation = orientation;
                }
                else
                {
                    ClientLocations[clientId] = new Entity(null, orientation, Globals.DEFAULT_SPEED, Globals.DEFAULT_SPEED);
                    m_parentInterface.onClientConnected(ClientLocations[clientId]);
                }
            }
        }

        private void removeClient(int clientId)
        {
            lock (ClientLocations)
            {
                // If this client exists
                if (ClientLocations.ContainsKey(clientId))
                {
                    m_parentInterface.onClientDisconnected(ClientLocations[clientId]);
                    ClientLocations.Remove(clientId);
                }
            }
        }

        // Handle recieving incoming messages
        private void handleIncomingMessages()
        {
            if (!Connection.connectionValid(m_connection))
                return;

            try
            {
                // If the last tcp packet has been received, make another task for the next one
                if (!m_connection.TcpReady)
                {
                    m_connection.TcpReady = true;
                    ConnectionPacketBundle bundle = new ConnectionPacketBundle(m_connection, m_connection.TcpSocket);
                    m_connection.TcpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(), SocketFlags.None,
                        handleReceivePackets, bundle);
                }

                // If the last udp packet has been received, make another task for the next one
                if (!m_connection.UdpReady)
                {
                    m_connection.UdpReady = true;
                    ConnectionPacketBundle bundle = new ConnectionPacketBundle(m_connection, m_connection.UdpSocket);
                    m_connection.UdpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(),
                        SocketFlags.None, handleReceivePackets, bundle);
                }
            }
            catch (SocketException)
            {
                closeConnection();
            }
            catch
            {
            }
        }

        private void handleHeartbeat()
        {
            // Heartbeat time
            if (m_parentInterface.getTime() <= m_lastHeartbeat + Globals.PACKET_HEARTBEAT_FREQUENCY) 
                return;

            lock (m_connection)
            {
                try
                {
                    byte[] packet = Packet.bundle(new HeartbeatPacket(ClientId));
                    m_connection.TcpSocket.Send(packet);
                    m_lastHeartbeat = m_parentInterface.getTime();
                }
                catch
                {
                    closeConnection();
                }
            }
        }

        private void handleReceivePackets(IAsyncResult ar)
        {
            ConnectionPacketBundle bundle = (ConnectionPacketBundle)ar.AsyncState;
            Socket socket = bundle.Socket;

            if (socket.ProtocolType == ProtocolType.Tcp)
                bundle.Connection.TcpReady = false;
            else if (socket.ProtocolType == ProtocolType.Udp)
                bundle.Connection.UdpReady = false;

            int bytesReceived = 0;

            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            catch
            {
                closeConnection();
            }

            // If we haven't received a proper packet, leave
            if (bytesReceived <= 0)
                return;

            Packet[] packets;

            try
            {
                packets = Packet.unbundle(bundle.RawBytes);

                if (socket.ProtocolType == ProtocolType.Tcp)
                    bundle.Connection.TcpLastReceived = m_parentInterface.getTime();
                else if (socket.ProtocolType == ProtocolType.Udp)
                    bundle.Connection.UdpLastReceived = m_parentInterface.getTime();
            }
            catch
            {
                // Failed to unbundle packet. Discard and return.
                return;
            }

            // Packets is now safe to read from, as any exception thrown during
            // unbundling will cause the execution to leave
            foreach (Packet packet in packets)
            {
                switch (packet.PacketType)
                {
                    case PacketType.CLIENT_CONNECT_PACKET:

                        ClientConnectPacket connectPacket = (ClientConnectPacket)packet;
                        updateClientPosition(connectPacket.ClientId, connectPacket.Orientation);
                        m_parentInterface.postClientConnectedToChat(connectPacket.ClientId);

                        break;

                    case PacketType.CHAT_PACKET:

                        ChatPacket chatPacket = (ChatPacket)packet;
                        m_parentInterface.clientChat(chatPacket.Name, chatPacket.Message);

                        break;

                    case PacketType.POSITION_PACKET:

                        PositionPacket positionPacket = (PositionPacket)packet;
                        updateClientPosition(positionPacket.ClientId, positionPacket.Orientation);
                        break;

                    case PacketType.CLIENT_DISCONNECT_PACKET:

                        ClientDisconnectPacket discoPacket = (ClientDisconnectPacket)packet;
                        removeClient(discoPacket.ClientId);
                        m_parentInterface.postClientDisconnectedToChat(discoPacket.ClientId);

                        break;
                }
            }
        }

        // Handle coordinating all clients
        private void handleOutgoingMessages()
        {
            Packet[] tcpQueue;
            Packet[] udpQueue;

            lock (m_tcpPacketQueue)
            {
                tcpQueue = m_tcpPacketQueue.ToArray();
            }

            lock (m_udpPacketQueue)
            {
                udpQueue = m_udpPacketQueue.ToArray();
            }

            // Check if the connection is valid (not null)
            if (!Connection.connectionValid(m_connection))
                return;

            // If there is anything in the tcp queue
            if (tcpQueue.Any())
                sendPacketQueue(m_connection.TcpSocket, tcpQueue, m_connection.ClientId, m_connection.RemoteEndPoint);

            // If there is anything in the udp queue
            if (udpQueue.Any())
                sendPacketQueue(m_connection.UdpSocket, udpQueue, m_connection.ClientId, m_connection.RemoteEndPoint);
        }

        // Sends packets from the provided queue to the provided socket
        // Client ID is for information output only
        private void sendPacketQueue(Socket socket, Packet[] queue, int clientId, EndPoint endPoint)
        {
            lock (queue)
            {
                try
                {
                    foreach (byte[] rawArray in queue.Select(Packet.bundle))
                    {
                        // Send the packets!
                        socket.SendTo(rawArray, endPoint);
                    }
                }
                catch
                {
                    closeConnection();
                }
            }
        }

        private void addPacketToQueue(List<Packet> queue, Packet packet)
        {
            lock (queue)
            {
                queue.Add(packet);
            }
        }

        private void clearQueue(List<Packet> queue)
        {
            lock (queue)
            {
                queue.Clear();
            }
        }
    }
}
