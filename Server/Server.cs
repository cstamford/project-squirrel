using System;
using System.Diagnostics;
using System.Net.Sockets;
using Squirrel.Data;
using Squirrel.Packets;

namespace Squirrel.Server
{
    public class Server
    {
        private const float UPDATES_PER_SECOND = 33.0f;
        private const float UPDATES_TICK_TIME = 1000.0f / UPDATES_PER_SECOND;
        private const int PACKET_BUFFER_SIZE = 1024;
        private const string SERVER_PREFIX = "[SERVER]: ";

        private readonly Stopwatch m_timer = new Stopwatch();
        private bool m_running = true;

        public void run()
        {
            m_timer.Start();
            write("Thread started");

            while (m_running)
            {
                // Make sure we only send out network packets once every UPDATES_TICK_TIME
                if (m_timer.ElapsedMilliseconds < UPDATES_TICK_TIME)
                    continue;

                // Acquire mutex lock on the connection list
                lock (Application.ActiveConnections)
                {
                    // Check for incoming messages
                    for (int i = 0; i < Application.ActiveConnections.Count; ++i)
                    {
                        handleIncomingMessages(Application.ActiveConnections[i]);
                        handleOutgoingMessages(Application.ActiveConnections[i]);
                        handleClientDisconnect(Application.ActiveConnections[i]);
                    }
                }

                // Restart the timer
                m_timer.Restart();
            }

            write("Thread ended");
        }

        // Handle recieving incoming messages
        public void handleIncomingMessages(Connection connection)
        {
            try
            {
                byte[] rawInput = new byte[PACKET_BUFFER_SIZE];
                connection.TcpSocket.Receive(rawInput);
                Packet packet = Packet.deserialize<Packet>(rawInput);
                write("" + packet.ToString());
            }
            catch (Exception e)
            {
                write(e.ToString());
            }
        }

        // Handle coordinating all clients
        public void handleOutgoingMessages(Connection connection)
        {
            
        }

        // Handle disconnecting clients who have left
        public void handleClientDisconnect(Connection connection)
        {

            try
            {
                // Just send an empty packet to make sure the connections are still alive
                connection.TcpSocket.Send(new byte[0]);
            }
            catch (Exception)
            {
                write("Client ID " + connection.ClientId + " dropped");

                connection.TcpSocket.Close();
                connection.UdpSocket.Close();
                connection.ClientId = -1;

                Application.ActiveConnections.Remove(connection);
            }
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
