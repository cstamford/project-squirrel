using System;
using System.IO;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace Squirrel.Packets
{
    [Serializable]
    public class Packet
    {
        private const int PACKET_BUFFER_SIZE = 1024;

        [NonSerialized] public byte[] Buffer;
        [NonSerialized] public Socket Socket;
        public PacketType PacketType { get; set; }
        public int ClientId { get; set; }

        public Packet()
        {
            Buffer = new byte[PACKET_BUFFER_SIZE];
        }

        public Packet(PacketType packetType, int clientId)
        {
            Buffer = new byte[PACKET_BUFFER_SIZE];
            PacketType = packetType;
            ClientId = clientId;
        }

        public override string ToString()
        {
            return "Packet: " + PacketType + " Client ID: " + ClientId;
        }

        public static byte[] bundle(Packet packet)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, packet);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        public static Packet unbundle(byte[] raw)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream(raw))
            {
                return (Packet)formatter.Deserialize(stream);
            }
        }
    }
}
