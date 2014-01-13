using System;
using System.Collections.Generic;
using Squirrel.Data;

namespace Squirrel.Server
{
    public class Server
    {
        private const string SERVER_PREFIX = "[SERVER]: ";
        private bool m_running = true;

        private static readonly List<Connection> m_activeConnections = new List<Connection>();

        public void run()
        {
            write("Thread started");

            while (m_running)
            {
                // Ping all active clients
                for (int i = 0; i < m_activeConnections.Count; ++i)
                {
                    Connection conn = m_activeConnections[i];

                    try
                    {
                        // Just send an empty packet to make sure the connections are still alive
                        conn.TcpSocket.Send(new byte[0]);
                    }
                    catch (Exception)
                    {
                        write("Client ID " + conn.ClientId + " dropped");

                        conn.TcpSocket.Close();
                        conn.UdpSocket.Close();
                        conn.ClientId = -1;

                        m_activeConnections.Remove(conn);
                    }
                }
            }

            write("Thread ended");
        }

        public void setRunning(bool state)
        {
            m_running = state;
        }

        public void addConnection(Connection newConnection)
        {
            m_activeConnections.Add(newConnection);
        }

        private void write(string input)
        {
            Console.WriteLine(SERVER_PREFIX + input);
        }
    }
}
