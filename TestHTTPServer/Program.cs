using System;
using LWHTTP.Server;

namespace LWHTTP
{
    public class Program
    {
        public static HTTPServer Server { get; private set; }

        static void Main(string[] args)
        {
            Server = new HTTPServer();
            Server.Initialize();
        }
    }
}
