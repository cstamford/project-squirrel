using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Squirrel.Data;

namespace Squirrel.Server
{
    public static class Application
    {
        public static List<Connection> ActiveConnections { get; private set; }
        public static Dictionary<int, Orientation> ClientLocations { get; set; }
        private static bool m_running = true;

        private static Listener m_listener;
        private static Thread m_listenerThread;

        private static Server m_server;
        private static Thread m_serverThread;

        private static void Main(string[] args)
        {
            ActiveConnections = new List<Connection>();

            Console.Title = "Project Squirrel Server";

            Console.WriteLine("###########################################");
            Console.WriteLine("#                                         #");
            Console.WriteLine("#            PROJECT SQUIRREL             #");
            Console.WriteLine("#                Server                   #");
            Console.WriteLine("#                                         #");
            Console.WriteLine("###########################################");
            Console.WriteLine();

            m_listener = new Listener(37500);

            m_listenerThread = new Thread(m_listener.run);
            m_listenerThread.Start();

            m_server = new Server();
            m_serverThread = new Thread(m_server.run);
            m_serverThread.Start();

            while (m_running)
            {
                Console.WriteLine("");
                // Get user input
                string input = Console.ReadLine();
                Console.WriteLine("");

                handleInput(input);
            }
        }

        private static void handleInput(string input)
        {

            // Continue with the loop if no input has been entered
            if (input == null)
                return;

            // Convert input to uppercase
            input = input.ToUpper();

            // Split input using whitespace as the delimiter
            string[] command = input.Split(new char[] { ' ' });

            int commandCount = command.Count();
            int clientId = 0;

            switch (command[0])
            {
                case "QUIT":

                    // Abort the thread -- it is blocking
                    m_listener.setRunning(false);
                    m_listenerThread.Abort();
                    m_listenerThread.Join();

                    // Join the server thread
                    m_server.setRunning(false);
                    m_serverThread.Join();

                    // Set the UI thread running state to off
                    m_running = false;

                    break;

                case "KICK":

                    if (commandCount == 1 || !int.TryParse(command[1], out clientId))
                    {
                        Console.WriteLine("Invalid syntax, no client ID entered");
                    }
                    else
                    {
                        Connection connection = getConnection(clientId);

                        if (connection != null)
                        {
                            closeConnection(connection);
                        }
                        else
                        {
                            Console.WriteLine("No client found with client ID " + clientId);
                        }
                    }

                    break;

                case "STATUS":

                    if (commandCount == 1)
                    {
                        if (ActiveConnections.Count == 0)
                        {
                            Console.WriteLine("    No clients connected");
                        }
                        else
                        {
                            // Acquire mutex lock on the connection list
                            lock (ActiveConnections)
                            {
                                foreach (Connection connection in ActiveConnections)
                                {
                                    Console.WriteLine("    " + connection);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (int.TryParse(command[1], out clientId))
                        {
                            // Acquire mutex lock on the connection list
                            Connection connection = getConnection(clientId);

                            if (connection != null)
                            {
                                Console.WriteLine("    " + connection);
                            }
                            else
                            {
                                Console.WriteLine("    No client found with client ID " + clientId);
                            }
                        }
                        else
                        {
                            Console.WriteLine("Invalid syntax, no client ID entered");
                        }
                    }

                    break;

                case "HELP":

                    Console.WriteLine("quit                       :     Closes the server");
                    Console.WriteLine("kick <clientID>            :     Kicks the provided client");
                    Console.WriteLine("status                     :     Displays the status of all connected clients");
                    Console.WriteLine("status <clientID>          :     Displays the status of provided client");

                    break;

                default:

                    Console.WriteLine("Invalid syntax");

                    break;
            }
        }

        public static void closeConnection(Connection connection)
        {
            Console.WriteLine("Client ID " + connection.ClientId + " dropped");

            connection.TcpSocket.Close();
            connection.TcpSocket = null;

            connection.UdpSocket.Close();
            connection.UdpSocket = null;

            connection.ClientId = -1;

            lock (ActiveConnections)
            {
                ActiveConnections.Remove(connection);
            }
        }

        public static Connection getConnection(int clientId)
        {
            lock (ActiveConnections)
            {
                return ActiveConnections.FirstOrDefault(it => it.ClientId == clientId);
            }
        }

        public static bool connectionValid(Connection connection)
        {
            return connection != null && connection.ClientId != -1 && connection.TcpSocket != null &&
                   connection.UdpSocket != null;
        }
    }
}
