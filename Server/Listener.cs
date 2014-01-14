using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Squirrel.Data;
using Squirrel.Packets;

namespace Squirrel.Server
{
    public class Listener
    {
        private const string LISTENER_PREFIX = "[LISTENER]: ";
        private readonly IPEndPoint m_endPoint;
        private readonly Socket m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool m_running = true;
        private readonly int m_port;

        public Listener(int port)
        {
            m_port = port;
            m_endPoint = new IPEndPoint(IPAddress.Any, m_port);

            try
            {
                // Allow us to bind UDP and TCP to the same port
                m_listener.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                // Bind the listener to the endpoint
                m_listener.Bind(m_endPoint);

                // Listen on the endpoint
                m_listener.Listen(m_port);
            }
            catch (Exception e)
            {
                write("Failed to bind to TCP port " + m_port);
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

                    // Blocks this thread until next connection - this is fine, because this is the listening thread
                    // It's not going to be doing anything else anyway.
                    connection.TcpSocket = m_listener.Accept();

                    // Creates a UDP socket to go with our TCP socket
                    connection.UdpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                    // Allow us to bind UDP and TCP to the same port
                    connection.UdpSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

                    // Bind!
                    connection.UdpSocket.Bind(m_endPoint);

                    write("Accepted connection from " + connection.TcpSocket.RemoteEndPoint);

                    bool assigned = false;
                    int clientId = 1;

                    // Acquire mutex lock on the connection list
                    lock (Application.ActiveConnections)
                    {
                        // Iterate through existing connections to determine a new client ID
                        for (int i = 0; !assigned && i < Application.ActiveConnections.Count; ++i)
                        {
                            // If there isn't a connection in this collection with the same client ID as our proposed
                            // client ID, assign it
                            if (Application.ActiveConnections.FirstOrDefault(it => it.ClientId == clientId) == null)
                                assigned = true;
                            else
                                clientId += 1;
                        }

                        if (!assigned)
                            clientId = Application.ActiveConnections.Count + 1;

                        // Allocate the client his ID
                        connection.ClientId = clientId;

                        // Generate a random location for the client to start at.
                        Random rng = new Random();

                        // Make sure the location is within game bounds
                        Orientation orientation = new Orientation(
                            (float)rng.NextDouble() * Globals.GAME_WIDTH, 
                            (float)rng.NextDouble() * Globals.GAME_HEIGHT, 
                            (float)rng.NextDouble() * 360.0f);

                        // Send the client his new ID and position
                        connection.TcpSocket.Send(
                            Packet.bundle(new NewClientPacket(clientId, orientation)));

                        // Set the last recived packet times to the current system time
                        connection.TcpLastReceived = Application.getTime();
                        connection.UdpLastReceived = Application.getTime();

                        // Add the connection
                        Application.ActiveConnections.Add(connection);

                        write("New connection " + connection.TcpSocket.RemoteEndPoint + " assigned client ID " +
                              connection.ClientId);
                    }
                }
            }
            catch (Exception e)
            {
                // This is caught if the thread is aborted, which only happens when the application is closing.
                write(e.ToString());
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
