using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Squirrel.Data;
using Squirrel.Packets;

namespace Squirrel.Server
{
    public class Server
    {
        private const string SERVER_PREFIX = "[SERVER]: ";

        private readonly Stopwatch m_timer = new Stopwatch();

        private readonly List<Packet> m_tcpPacketQueue = new List<Packet>();
        private readonly List<Packet> m_udpPacketQueue = new List<Packet>(); 

        private bool m_running = true;

        public void run()
        {
            m_timer.Start();

            write("Thread started");

            while (m_running)
            {
                // Acquire mutex lock on the connection list
                lock (Application.ActiveConnections)
                {
                    // Handle incoming messages. This should not be throttled, unlike outgoing and heartbeat messages.
                    handleIncomingMessages();
                    
                    // Send out heartbeat to clients
                    handleClientDisconnect();

                    // Make sure we only send out network packets once every UPDATES_TICK_TIME
                    if (m_timer.ElapsedMilliseconds < Globals.UPDATES_TICK_TIME)
                        continue;

                    // Add the list to the UDP queue
                    addPacketsToQueue(m_udpPacketQueue, getPositionPacketList());

                    // Handle outgoing messages
                    handleOutgoingMessages();

                    // Clear the queues
                    clearQueue(m_tcpPacketQueue);
                    clearQueue(m_udpPacketQueue);

                    // Restart the timer
                    m_timer.Restart();
                }
            }

            write("Thread ended");
        }

        public void clientConnected(int clientId, Orientation orientation)
        {
            lock (m_tcpPacketQueue)
            {
                m_tcpPacketQueue.Add(new ClientConnectPacket(clientId, orientation));
            }
        }

        public void clientDisconnected(int clientId)
        {
            lock (m_tcpPacketQueue)
            {
                m_tcpPacketQueue.Add(new ClientDisconnectPacket(clientId));
            }
        }

        public void setRunning(bool state)
        {
            m_running = state;
        }

        // Handle recieving incoming messages
        private void handleIncomingMessages()
        {
            for (int i = 0; i < Application.ActiveConnections.Count; ++i)
            {
                Connection connection = Application.ActiveConnections.ElementAtOrDefault(i);

                if (!Connection.connectionValid(connection))
                    continue;

                try
                {
                    // If the last tcp packet has been received, make another task for the next one
                    if (!connection.TcpReady)
                    {
                        connection.TcpReady = true;
                        ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, connection.TcpSocket);
                        connection.TcpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(), SocketFlags.None,
                            handleReceivePackets, bundle);
                    }

                    // If the last udp packet has been received, make another task for the next one
                    if (!connection.UdpReady)
                    {
                        connection.UdpReady = true;
                        ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, connection.UdpSocket);
                        connection.UdpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(),
                            SocketFlags.None, handleReceivePackets, bundle);
                    }
                }
                catch (SocketException exception)
                {
                    write(exception);
                    Application.closeConnection(connection);
                }
                catch (Exception exception)
                {
                    write(exception);
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

            for (int i = 0; i < Application.ActiveConnections.Count; ++i)
            {
                Connection connection = Application.ActiveConnections.ElementAtOrDefault(i);

                // Check if the connection is valid (not null)
                if (!Connection.connectionValid(connection))
                    return;

                // If there is anything in the tcp queue
                if (tcpQueue.Any())
                    sendPacketQueue(connection.TcpSocket, tcpQueue, connection.ClientId, connection.RemoteEndPoint);

                // If there is anything in the udp queue
                if (udpQueue.Any())
                    sendPacketQueue(connection.UdpSocket, udpQueue, connection.ClientId, connection.RemoteEndPoint);
            }
        }

        // Sends packets from the provided queue to the provided socket
        // Client ID is for information output only
        private void sendPacketQueue(Socket socket, Packet[] queue, int clientId, EndPoint endPoint)
        {
            lock (queue)
            {
                // Don't send packets to the client where it originiated from, unless it's a chat packet
                foreach (Packet packet in queue.Where(packet => packet.ClientId != clientId || packet.PacketType == PacketType.CHAT_PACKET))
                {
                    byte[] rawArray = Packet.bundle(packet);

                    try
                    {
                        // Send the packets!
                        socket.SendTo(rawArray, endPoint);
                        write("Sent " + socket.ProtocolType + " packet " + packet.PacketType + 
                            " concerning Client ID " + packet.ClientId + " to Client ID " + clientId, LogVerbosity.LOG_DEBUG);
                    }
                    catch
                    {
                        // This catches exceptions when sending packets to sockets
                        // which have been closed by the client.
                        // We don't need to handle this because the heartbeat will remove
                        // the disconnected client after the timeout delay has passed.
                    }
                }
            }
        }

        // Handle disconnecting clients who have left
        private void handleClientDisconnect()
        {
            for (int i = 0; i < Application.ActiveConnections.Count; ++i)
            {
                Connection connection = Application.ActiveConnections.ElementAtOrDefault(i);

                if (!Connection.connectionValid(connection))
                    continue;

                if (Application.getTime() > connection.TcpLastReceived + Globals.PACKET_TIME_OUT)
                {
                    write(connection.ToString() + " timed out", LogVerbosity.LOG_VERBOSE);
                    Application.closeConnection(connection);
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
                // This exception doesn't need to be handled - it's just complaining
                // about disposed sockets due to a client disconnect / kick
            }

            // If we haven't received a proper packet, leave
            if (bytesReceived <= 0) 
                return;

            Packet[] packets;

            try
            {
                packets = Packet.unbundle(bundle.RawBytes);

                for (int i = 0; i < packets.Count(); ++i)
                {
                    write("<" + i + "> Received " + socket.ProtocolType + " " + packets[i].ToString(), LogVerbosity.LOG_DEBUG);
                }

                if (socket.ProtocolType == ProtocolType.Tcp)
                    bundle.Connection.TcpLastReceived = Application.getTime();
                else if (socket.ProtocolType == ProtocolType.Udp)
                    bundle.Connection.UdpLastReceived = Application.getTime();
            }
            catch
            {
                write("Failed to unbundle packet", LogVerbosity.LOG_VERBOSE);
                return;
            }

            // Packets is now safe to read from, as any exception thrown during
            // unbundling will cause the execution to leave
            foreach (Packet packet in packets)
            {
                switch (packet.PacketType)
                {
                    case PacketType.CHAT_PACKET:

                        ChatPacket chatPacket = (ChatPacket) packet;
                        addPacketToQueue(m_tcpPacketQueue, chatPacket);
                        write(chatPacket.Name + ": " + chatPacket.Message, LogVerbosity.LOG_MINIMAL);
                        break;

                    case PacketType.POSITION_PACKET:

                        PositionPacket positionPacket = (PositionPacket) packet;
                        Application.updateClientLocation(packet.ClientId, positionPacket.Orientation);
                        break;
                }
            }

        }

        private List<Packet> getPositionPacketList()
        {
           return Application.ClientLocations.Select(pair => new PositionPacket(pair.Key, pair.Value)).Cast<Packet>().ToList();
        }

        private void addPacketToQueue(List<Packet> queue, Packet packet)
        {
            lock (queue)
            {
                queue.Add(packet);
            }
        }

        private void addPacketsToQueue(List<Packet> queue, IEnumerable<Packet> packet)
        {
            lock (queue)
            {
                queue.AddRange(packet);
            }
        }

        private void clearQueue(List<Packet> queue)
        {
            lock (queue)
            {
                queue.Clear();
            }
        }

        private static void write(string input, LogVerbosity verbosity = LogVerbosity.LOG_NORMAL)
        {
            if (verbosity <= Application.LogLevel)
                Console.WriteLine(SERVER_PREFIX + input);
        }

        private static void write(Exception exception, LogVerbosity verbosity = LogVerbosity.LOG_NORMAL)
        {
            // If the verbosity is lower than the provided verbosity, leave
            if (Application.LogLevel < verbosity)
                return;

            // Otherwise, display exception details based on the verbosity
            if (Application.LogLevel >= LogVerbosity.LOG_DEBUG)
            {
                Console.WriteLine(SERVER_PREFIX + exception);
            }
            else if (Application.LogLevel >= LogVerbosity.LOG_VERBOSE)
            {
                Console.WriteLine(SERVER_PREFIX + exception.Message);
            }
        }
    }
}
