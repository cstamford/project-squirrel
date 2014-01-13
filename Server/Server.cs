using System;
using System.Collections.Generic;
using Squirrel.Data;

namespace Squirrel.Server
{
    public class Server
    {
        private const string SERVER_PREFIX = "[SERVER]: ";
        private bool m_running = true;

        public void run()
        {
            write("Thread started");

            while (m_running)
            {
                // Acquire mutex lock on the connection list
                lock (Application.ActiveConnections)
                {
                    // Ping all active clients
                    for (int i = 0; i < Application.ActiveConnections.Count; ++i)
                    {
                        Connection conn = Application.ActiveConnections[i];

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

                            Application.ActiveConnections.Remove(conn);
                        }
                    }
                }
            }

            write("Thread ended");
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
