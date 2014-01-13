using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Squirrel.Packets
{
    [Serializable]
    public class Packet
    {
        public PacketType PacketType { get; set; }
        public int ClientId { get; set; }

        public Packet() {  }

        public Packet(PacketType packetType, int clientId)
        {
            PacketType = packetType;
            ClientId = clientId;
        }

        public override string ToString()
        {
            return "Packet: " + PacketType + " Client ID: " + ClientId;
        }

        public static byte[] serialize<T>(T packet)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                formatter.Serialize(stream, packet);
                stream.Seek(0, SeekOrigin.Begin);
                return stream.ToArray();
            }
        }

        public static T deserialize<T>(byte[] raw)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream(raw))
            {
                return (T)formatter.Deserialize(stream);
            }
        }

        public static PacketType getPacketType(byte[] raw)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream(raw))
            {
                return ((Packet) formatter.Deserialize(stream)).PacketType;
            }
        }
    }
}
