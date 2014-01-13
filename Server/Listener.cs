using System;
using System.Net;
using System.Net.Sockets;
using Squirrel.Data;

namespace Squirrel.Server
{
    public class Listener
    {
        private const string LISTENER_PREFIX = "[LISTENER]: ";

        private bool m_running = true;
        private readonly Socket m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private readonly int m_port;

        public Listener(int port)
        {
            m_port = port;

            try
            {
                m_listener.Bind(new IPEndPoint(IPAddress.Any, m_port));
                m_listener.Listen(m_port);
            }
            catch (Exception e)
            {
                write("Failed to bind to UDP port " + m_port);
                write(e.ToString());
            }
        }

        public void run()
        {
            write("Thread started, listening on port " + m_port);

            try
            {
                while (m_running)
                {
                    Connection connection = new Connection();

                    // Blocks this thread until next connection - this is fine, because this is the listenin thread
                    connection.TcpSocket = m_listener.Accept();

                    // Creates a UDP socket to go with our TCP socket
                    connection.UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                    connection.UdpSocket.Bind(connection.TcpSocket.RemoteEndPoint);

                    // The server thread will assign a client ID
                    connection.ClientId = -1;

                    write("Accepted connection from " + connection.TcpSocket.RemoteEndPoint);

                    // Pass the new connection to the application
                    Application.addConnection(connection);
                }
            }
            catch (Exception)
            {
            }

            write("Thread ended");
        }

        public void setRunning(bool state)
        {
            m_running = state;

            if (!state)
                m_listener.Close();
        }

        private static void write(string input)
        {
            Console.WriteLine(LISTENER_PREFIX + input);
        }
    }
}
