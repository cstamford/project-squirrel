namespace Squirrel
{
    public static class Globals
    {
        public const int GAME_WIDTH = 800;
        public const int GAME_HEIGHT = 600;

        public const int PACKET_BUFFER_SIZE = 1024;
        public const int PACKET_HEARTBEAT_FREQUENCY = 1000;

        public const int CLIENT_TIME_OUT = 2000;

        public const float NETWORK_UPDATES_PER_SECOND = 1.0f;
        public const float NETWORK_UPDATES_TICK_TIME = 1000.0f / NETWORK_UPDATES_PER_SECOND;

        public const float GAME_UPDATES_PER_SECOND = 60.0f;
        public const float GAME_UPDATES_TICK_TIME = 1000.0f / GAME_UPDATES_PER_SECOND;

        public const float DEFAULT_SPEED = 5.0f;
    }
}
