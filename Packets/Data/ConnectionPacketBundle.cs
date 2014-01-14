using Squirrel.Packets;

namespace Squirrel.Data
{
    public class ConnectionPacketBundle
    {
        public Connection Connection { get; set; }
        public Packet Packet { get; set; }

        public ConnectionPacketBundle(Connection connection, Packet packet)
        {
            Connection = connection;
            Packet = packet;
        }
    }
}
