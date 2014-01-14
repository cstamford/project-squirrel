using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Squirrel.Data;
using Squirrel.Packets;

namespace Squirrel.Server
{
    public class Server
    {
        private const float UPDATES_PER_SECOND = 33.0f;
        private const float UPDATES_TICK_TIME = 1000.0f / UPDATES_PER_SECOND;
        private const string SERVER_PREFIX = "[SERVER]: ";

        private readonly Stopwatch m_timer = new Stopwatch();
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
                    for (int i = 0; i < Application.ActiveConnections.Count; ++i)
                    {
                        // Handle incoming messages. This should not be throttled, unlike outgoing and heartbeat messages.
                        handleIncomingMessages(Application.ActiveConnections.ElementAtOrDefault(i));

                        // Make sure we only send out network packets once every UPDATES_TICK_TIME
                        if (m_timer.ElapsedMilliseconds < UPDATES_TICK_TIME)
                            continue;

                        // Handle outgoing messages
                        handleOutgoingMessages(Application.ActiveConnections.ElementAtOrDefault(i));

                        // Send out heartbeat to clients
                        handleClientDisconnect(Application.ActiveConnections.ElementAtOrDefault(i));
                    }
                }

                // Restart the timer
                m_timer.Restart();
            }

            write("Thread ended");
        }

        // Handle recieving incoming messages
        private void handleIncomingMessages(Connection connection)
        {
            if (!Application.connectionValid(connection))
                return;

            try
            {
                // If the last tcp packet has been received, make another task for the next one
                if (!connection.TcpReady)
                {
                    connection.TcpReady = true;
                    Packet tcpPacket = new Packet {Socket = connection.TcpSocket};
                    ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, tcpPacket);
                    connection.TcpSocket.BeginReceive(tcpPacket.Buffer, 0, tcpPacket.Buffer.Count(), SocketFlags.None,
                        onReceiveTcp, bundle);
                }

                // If the last udp packet has been received, make another task for the next one
                if (!connection.UdpReady)
                {
                    connection.UdpReady = true;
                    Packet udpPacket = new Packet {Socket = connection.UdpSocket};
                    ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, udpPacket);
                    connection.UdpSocket.BeginReceive(udpPacket.Buffer, 0, udpPacket.Buffer.Count(), SocketFlags.None,
                        onReceiveUdp, bundle);
                }
            }
            catch (Exception e)
            {
                write(e.ToString());
                Application.closeConnection(connection);
            }
        }

        // Handle coordinating all clients
        private void handleOutgoingMessages(Connection connection)
        {
            if (!Application.connectionValid(connection))
                return;


        }

        // Handle disconnecting clients who have left
        private void handleClientDisconnect(Connection connection)
        {
            if (!Application.connectionValid(connection))
                return;

            try
            {
                // Just send an empty packet to make sure the connections are still alive
                connection.TcpSocket.Send(new byte[0]);
            }
            catch (Exception)
            {
                Application.closeConnection(connection);
            }
        }

        private void onReceiveTcp(IAsyncResult ar)
        {
            ConnectionPacketBundle bundle = (ConnectionPacketBundle)ar.AsyncState;
            Packet packet = bundle.Packet;
            Socket socket = packet.Socket;
            int bytesReceived = 0;

            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            catch (Exception e)
            {
                write(e.ToString());
            }

            if (bytesReceived > 0)
            {
                try
                {
                    packet = Packet.unbundle(packet.Buffer);
                    write("Received TCP " + packet.ToString());
                }
                catch (Exception e)
                {
                    write("Failed to unbundle packet");
                }
            }
            
            bundle.Connection.TcpReady = false;
        }

        private void onReceiveUdp(IAsyncResult ar)
        {
            ConnectionPacketBundle bundle = (ConnectionPacketBundle)ar.AsyncState;
            Packet packet = bundle.Packet;
            Socket socket = packet.Socket;
            int bytesReceived = 0;

            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            catch (Exception e)
            {
                write(e.ToString());
            }

            if (bytesReceived > 0)
            {
                try
                {
                    packet = Packet.unbundle(packet.Buffer);
                    write("Received UDP " + packet.ToString());
                }
                catch (Exception)
                {
                    write("Failed to unbundle packet");
                }
            }

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
