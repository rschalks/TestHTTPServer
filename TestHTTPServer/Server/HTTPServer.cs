using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using LWHTTP.Utils;
using LWHTTP.Utils.XML;
using LWHTTP.HTTPCommon;
using System.IO;

namespace LWHTTP.Server
{
    public class HTTPClient
    {
        private byte[] _buffer;

        public IPAddress IPAddress { get; set; }
        public Socket Socket { get; set; }
        public const int BufferSize = 1024;
        public byte[] Buffer { get { return this._buffer; } }

        public HTTPClient()
        {
            this._buffer = new byte[BufferSize];
        }
    }

    public class HTTPServer
    {
        #region Private members
        private XMLConfig _config;
        private Socket _socket;
        #endregion

        public HTTPVersion SupportedVersion { get; private set; }

        #region Public methods
        public void Initialize()
        {
            this.SupportedVersion = HTTPVersion.HTTP11;
            _config = new XMLConfig();
            _config.Initialize();

            var hostName = Dns.GetHostName();
            Console.WriteLine("Using hostName {0}...", hostName);
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < ipHostEntry.AddressList.Length; i++)
            {
                var ip = ipHostEntry.AddressList[i];
            }
            IPAddress ipAddr = ConsoleMenu.SelectFromArray<IPAddress>(ipHostEntry.AddressList, null);
            Console.WriteLine("Insert the port number.");
            int port = ConsoleMenu.GetInt32();
            Console.WriteLine("Insert the max. number of allowed connections.");
            int maxConn = ConsoleMenu.GetInt32();
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            Console.WriteLine("IP Address: {0}", ipAddr.ToString());
            Console.WriteLine("Port: {0}", port);
            Console.WriteLine("Max connections: {0}", maxConn);

            _socket = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            Console.WriteLine("Socket made.");
            try
            {
                _socket.Bind(ipEndPoint);
                Console.WriteLine("Socket bound");

                _socket.Listen(maxConn);
                Console.WriteLine("Socket is listening");

                String data = null;
                byte[] bytes;

                bytes = new byte[2048];
                while (true)
                {
                    Console.WriteLine("Waiting for connection...");
                    Socket handler = _socket.Accept();
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.ASCII.GetString(bytes, 0, bytesRec);
                    Console.WriteLine("received data: \n{0}", data);

                    HTTPMessage msg = HTTPMessage.Parse(data);

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void ReadConfig()
        {
            
        }
        #endregion

        #region Private methods

        private void AcceptCallback(IAsyncResult asyncResult)
        {
            Console.WriteLine("Accept Callback called.");
        }
        #endregion
    }
}
