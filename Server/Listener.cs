﻿using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using Squirrel.Data;

namespace Squirrel.Server
{
    public class Listener
    {
        private const string LISTENER_PREFIX = "[LISTENER]: ";
        private readonly Socket m_listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        private bool m_running = true;
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
                    connection.UdpSocket.Bind(connection.TcpSocket.RemoteEndPoint);

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

                        connection.ClientId = clientId;

                        // Add the connection
                        Application.ActiveConnections.Add(connection);

                        write("New connection " + connection.TcpSocket.RemoteEndPoint + " assigned client ID " +
                              connection.ClientId);
                    }
                }
            }
            catch (Exception)
            {
                // This is caught if the thread is aborted, which only happens when the application is closing.
                write("Thread ended");
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