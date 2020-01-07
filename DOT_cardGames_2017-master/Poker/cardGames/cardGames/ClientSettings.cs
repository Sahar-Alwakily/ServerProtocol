using System.Net;

namespace cardGames
{
    public class ClientSettings
    {
        public static IPAddress Host => IPAddress.Parse("127.0.0.1");

        public static int Port => int.Parse("4242");

        public static int Size => int.Parse("4096");
    }
}