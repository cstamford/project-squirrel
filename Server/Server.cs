using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
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

                    // Handle outgoing messages
                    handleOutgoingMessages();

                    lock (m_tcpPacketQueue)
                    {
                        m_tcpPacketQueue.Clear();
                    }

                    lock (m_udpPacketQueue)
                    {
                        m_udpPacketQueue.Clear();
                    }

                    m_timer.Restart();
                }
            }

            write("Thread ended");
        }

        public void clientDisconnected(int clientId)
        {
            lock (m_tcpPacketQueue)
            {
                m_tcpPacketQueue.Add(new ClientDisconnectPacket(clientId));
            }
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
                }
                catch (Exception exception)
                {
                    write("TCP exception");
                    write(exception.Message);
                }

                try
                {
                    // If the last udp packet has been received, make another task for the next one
                    if (!connection.UdpReady)
                    {
                        EndPoint endpoint = connection.TcpSocket.RemoteEndPoint;

                        connection.UdpReady = true;
                        ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, connection.UdpSocket);
                        connection.UdpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(),
                            SocketFlags.None, handleReceivePackets, bundle);
                    }
                }
                catch (Exception exception)
                {
                    write("UDP exception");
                    write(exception.Message);
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

                //connection.TcpSocket.Send(Packet.bundle(new Packet()));

                connection.UdpSocket.SendTo(Packet.bundle(new Packet()), connection.TcpSocket.RemoteEndPoint);

                // If there is anything in the tcp queue
                if (tcpQueue.Any())
                    sendPacketQueue(connection.TcpSocket, tcpQueue, connection.ClientId);

                // If there is anything in the udp queue
                if (udpQueue.Any())
                    sendPacketQueue(connection.UdpSocket, udpQueue, connection.ClientId);
            }
        }

        // Sends packets from the provided queue to the provided socket
        // Client ID is for information output only
        private void sendPacketQueue(Socket socket, Packet[] queue, int clientId)
        {
            byte[] rawArray = Packet.bundle(queue);

            try
            {
                // Send TCP packets in the queue
                socket.Send(rawArray, rawArray.Count(), SocketFlags.None);
            }
            catch
            {
                // This catches exceptions when sending packets to sockets
                // which have been closed by the client.
                // We don't need to handle this because the heartbeat will remove
                // the disconnected client after the timeout delay has passed.
            }
            finally
            {
                write("Sent " + queue.Count() + " " + socket.ProtocolType + " packets to Client ID " + clientId);
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
                    write(connection.ToString() + " timed out");
                    Application.closeConnection(connection);
                }
            }
        }

        private void handleReceivePackets(IAsyncResult ar)
        {
            ConnectionPacketBundle bundle = (ConnectionPacketBundle)ar.AsyncState;
            Socket socket = bundle.Socket;

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

            if (bytesReceived > 0)
            {
                try
                {
                    Packet[] packets = Packet.unbundle(bundle.RawBytes);

                    for (int i = 0; i < packets.Count(); ++i)
                    {
                        write("<" + i + "> Received " + socket.ProtocolType + " " + packets[i].ToString());
                    }

                    if (socket.ProtocolType == ProtocolType.Tcp)
                        bundle.Connection.TcpLastReceived = Application.getTime();
                    else if (socket.ProtocolType == ProtocolType.Udp)
                        bundle.Connection.UdpLastReceived = Application.getTime();
                }
                catch
                {
                    write("Failed to unbundle packet");
                }
            }

            if (socket.ProtocolType == ProtocolType.Tcp)
                bundle.Connection.TcpReady = false;
            else if (socket.ProtocolType == ProtocolType.Udp)
                bundle.Connection.UdpReady = false;
        }

        public void setRunning(bool state)
        {
            m_running = state;
        }

        private void write(string input)
        {
            Console.WriteLine(SERVER_PREFIX + input);
        }
    }
}
