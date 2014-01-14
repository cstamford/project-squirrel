using System;
using System.Diagnostics;
using System.Linq;
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
        private readonly Stopwatch m_globalTimer = new Stopwatch();
        private bool m_running = true;

        public void run()
        {
            m_timer.Start();
            m_globalTimer.Start();

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
                        if (m_timer.ElapsedMilliseconds < Globals.UPDATES_TICK_TIME)
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
                    ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, connection.TcpSocket);
                    connection.TcpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(), SocketFlags.None,
                        onReceiveTcp, bundle);
                }

                // If the last udp packet has been received, make another task for the next one
                if (!connection.UdpReady)
                {
                    connection.UdpReady = true;
                    ConnectionPacketBundle bundle = new ConnectionPacketBundle(connection, connection.UdpSocket);
                    connection.UdpSocket.BeginReceive(bundle.RawBytes, 0, bundle.RawBytes.Count(), SocketFlags.None,
                        onReceiveUdp, bundle);
                }
            }
            catch (Exception e)
            {
                write(e.Message);
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

            if (m_globalTimer.ElapsedMilliseconds > connection.TcpLastReceived + Globals.PACKET_TIME_OUT)
            {
                write(connection.ToString() + " timed out");
                Application.closeConnection(connection);
            }
        }

        private void onReceiveTcp(IAsyncResult ar)
        {
            ConnectionPacketBundle bundle = (ConnectionPacketBundle)ar.AsyncState;
            Socket socket = bundle.Socket;
            int bytesReceived = 0;

            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            // This exception doesn't need to be handled - it's just complaining
            // about disposed sockets due to a client disconnect / kick
            catch { }

            if (bytesReceived > 0)
            {
                try
                {
                    Packet[] packets = Packet.unbundle(bundle.RawBytes);

                    for (int i = 0; i < packets.Count(); ++i)
                    {
                        write("<" + i + "> Received TCP " + packets[i].ToString());
                    }
                }
                catch
                {
                    write("Failed to unbundle packet");
                }
            }
            
            bundle.Connection.TcpReady = false;
        }

        private void onReceiveUdp(IAsyncResult ar)
        {
            ConnectionPacketBundle bundle = (ConnectionPacketBundle)ar.AsyncState;
            Socket socket = bundle.Socket;
            int bytesReceived = 0;

            try
            {
                bytesReceived = socket.EndReceive(ar);
            }
            // This exception doesn't need to be handled - it's just complaining
            // about disposed sockets due to a client disconnect / kick
            catch { }

            if (bytesReceived > 0)
            {
                try
                {
                    Packet[] packets = Packet.unbundle(bundle.RawBytes);

                    for (int i = 0; i < packets.Count(); ++i)
                    {
                        write("<" + i + "> Received UDP " + packets[i].ToString());
                    }
                }
                catch
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
