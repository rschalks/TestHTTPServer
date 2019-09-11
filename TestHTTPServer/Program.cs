using System;
using LWHTTP.Server;
using System.IO;

namespace LWHTTP
{
    public class Program
    {
        public static HTTPServer Server { get; private set; }

        static void Main(string[] args)
        {
            Server = new HTTPServer();
            string path = System.Reflection.Assembly.GetExecutingAssembly().Location;
            int indexOfSlash = path.LastIndexOf('\\');
            Server.HTTPPath = path.Substring(0, indexOfSlash) + "\\http";
            Server.Initialize();
        }
    }
}
