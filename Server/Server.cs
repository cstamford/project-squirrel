using System;
using System.Diagnostics;
using Squirrel.Data;

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
                // Make sure we only send out network packets once every UPDATES_TICK_TIME
                if (m_timer.ElapsedMilliseconds < UPDATES_TICK_TIME)
                    continue;

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

                // Restart the timer
                m_timer.Restart();
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
