using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using LWHTTP.Utils;
using LWHTTP.Utils.XML;
using LWHTTP.HTTPCommon;
using LWHTTP.HTTPCommon.HTTPHeaders;
using System.Threading;
using System.IO;

namespace LWHTTP.Server
{
    public class HTTPClient
    {
        private byte[] _inBuffer;
        private byte[] _outBuffer;

        public Socket Socket { get; set; }
        public const int BufferSize = 1024;
        public byte[] InBuffer { get { return this._inBuffer; } }
        public byte[] OutBuffer { get { return this._outBuffer; } }
        public StringBuilder StringBuilder { get; set; }

        public HTTPClient()
        {
            this.StringBuilder = new StringBuilder();
            this._inBuffer = new byte[BufferSize];
            this._outBuffer = new byte[BufferSize];
        }
    }

    class HTTPSocketListener
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);

        public HTTPSocketListener()
        {

        }

        public void StartListening()
        {
            var hostName = Dns.GetHostName();
            Console.WriteLine("Using hostName {0}...", hostName);
            IPHostEntry ipHostEntry = Dns.GetHostEntry(hostName);
            for (int i = 0; i < ipHostEntry.AddressList.Length; i++)
            {
                var ip = ipHostEntry.AddressList[i];
            }
            IPAddress ipAddr = ConsoleMenu.SelectFromArray<IPAddress>(ipHostEntry.AddressList, null);
            Console.WriteLine("Insert the port number.");
            int port = ConsoleMenu.GetInt32(0, Int32.MaxValue);
            Console.WriteLine("Insert the max. number of allowed connections.");
            int maxConn = ConsoleMenu.GetInt32(0, 100);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
            Console.WriteLine("IP Address: {0}", ipAddr.ToString());
            Console.WriteLine("Port: {0}", port);
            Console.WriteLine("Max connections: {0}", maxConn);

            Socket listener = new Socket(ipAddr.AddressFamily,
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                listener.Bind(ipEndPoint);
                listener.Listen(maxConn);

                while (true)
                {
                    allDone.Reset();
                    Console.WriteLine("Waiting for connection...");
                    listener.BeginAccept(new AsyncCallback(AcceptCallback), listener);

                    allDone.WaitOne();
                }
            }
            catch
            (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public void AcceptCallback(IAsyncResult er)
        {
            Console.WriteLine("Accepted.");
            allDone.Set();
            Socket listener = (Socket)er.AsyncState;
            Socket handler = listener.EndAccept(er);

            HTTPClient client = new HTTPClient();
            client.Socket = handler;
            Console.WriteLine("Beginning receive...");
            handler.BeginReceive(client.InBuffer, 0, HTTPClient.BufferSize, 0,
                new AsyncCallback(ReadCallback), client);
        }

        public void ReadCallback(IAsyncResult er)
        {
            Console.WriteLine("Ending receive.");
            String content = string.Empty;

            HTTPClient client = (HTTPClient)er.AsyncState;
            Socket handler = client.Socket;

            int bytesRead = handler.EndReceive(er);

            if (bytesRead > 0)
            {
                Console.WriteLine("Read {0} bytes.", bytesRead);

                client.StringBuilder.Append(Encoding.ASCII.GetString(client.InBuffer,
                    0, bytesRead));

                content = client.StringBuilder.ToString();
                Console.WriteLine("Received data: \n{0}", content);
            }

            HTTPMessage request = HTTPMessage.Parse(client.StringBuilder.ToString());
            HTTPMessage response = Program.Server.HandleHTTPRequest(request);
            int sent = SendResponse(er, response);
        }

        public int SendResponse(IAsyncResult er, HTTPMessage response)
        {
            HTTPClient client = (HTTPClient)er.AsyncState;
            byte[] buff = response.ToByteArray();
            Console.WriteLine(Utils.Utils.HexDump(buff));
            return client.Socket.Send(buff);
        }
    }

    public class HTTPServer
    {
        #region Private members
        private XMLConfig _config;
        private Socket _socket;
        private HTTPSocketListener _listener;
        #endregion

        public HTTPVersion SupportedVersion { get; private set; }
        public string HTTPPath { get; set; }

        #region Public methods
        public void Initialize()
        {
            this.SupportedVersion = HTTPVersion.HTTP11;
            _config = new XMLConfig();
            _config.Initialize();

            string kak = HTTPServer.EncodeURL("http://www.renkak.nl/1234567890-=!@#$%^&*()_+[],.<>");

            _listener = new HTTPSocketListener();
            _listener.StartListening();

            Console.WriteLine("Back to HTTPServer.Initialize");
        }

        public string LocalPathToAbsolute(string path)
        {
            string absolutePath = Program.Server.HTTPPath;
            path = path.Replace("/", "\\");
            if (path.EndsWith("\\"))
            {
                absolutePath += "\\index.html";
            }
            else
            {
                absolutePath += path;
            }

            return absolutePath;
        }

        public HTTPMessage HandleHTTPRequest(HTTPMessage request)
        {
            string path = LocalPathToAbsolute(request.Path);
            int offset = 0;
            HTTPMessage ret = new HTTPMessage();
            ret.IsResponse = true;
            if (!File.Exists(path))
            {
                ret.StatusCode = HTTPStatusCode.NotFound;
            }

            HTTPStatusCodeCategory category = HTTPHelpers.GetStatusCategory(ret.StatusCode);

            if (category == HTTPStatusCodeCategory.ClientError || 
                category == HTTPStatusCodeCategory.ServerError)
            {
                CreateErrorPage(ret);
                return ret;
            }

            IEnumerable<byte> fileContent = GetFile(path, ret);
            if (fileContent != null)
            {
                ret.Content = fileContent;
            }

            return ret;
        }

        public byte[] GetFile(string path, HTTPMessage response)
        {
            if (!File.Exists(path))
            {
                response.StatusCode = HTTPStatusCode.NotFound;
                return null;
            }

            return File.ReadAllBytes(path);
        }

        public void CreateIndexOfPage(HTTPMessage msg)
        {
            StringBuilder sb = new StringBuilder();
        }

        public void CreateErrorPage(HTTPMessage msg)
        {
            HTTPConnectionHeader connHeader = (HTTPConnectionHeader)msg.Headers.FirstOrDefault(h => h.GetType() == typeof(HTTPConnectionHeader));
            if (connHeader == null)
            {
                connHeader = new HTTPConnectionHeader()
                {
                    Content = HTTPConnection.KeepAlive
                };
                msg.Headers.Add(connHeader);
            }
            else
            {
                connHeader.Content = HTTPConnection.Close;
            }
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("<html><title>Error</title><h1>{0} {1}</h1></html>", ((int)msg.StatusCode).ToString(), HTTPHelpers.HTTPStatusCodeToString(msg.StatusCode));

            int len = sb.ToString().Length;

            HTTPContentLengthHeader lengthHeader = (HTTPContentLengthHeader)msg.Headers.FirstOrDefault(h => h.GetType() == typeof(HTTPContentLengthHeader));
            if (lengthHeader == null)
            {
                lengthHeader = new HTTPContentLengthHeader()
                {
                    Content = len
                };
                msg.Headers.Add(lengthHeader);
            }
            else
            {
                lengthHeader.Content = len;
            }

            HTTPContentTypeHeader typeHeader = (HTTPContentTypeHeader)msg.Headers.FirstOrDefault(h => h.GetType() == typeof(HTTPContentTypeHeader));
            if (typeHeader == null)
            {
                typeHeader = new HTTPContentTypeHeader()
                {
                    Content = new HTTPMIMEType()
                    {
                        Type = "text",
                        Subtype = "html"
                    }
                };
                msg.Headers.Add(typeHeader);
            }
            else
            {
                typeHeader.Content = new HTTPMIMEType()
                {
                    Type = "text",
                    Subtype = "html"
                };
            }

            byte[] buff = Encoding.ASCII.GetBytes(sb.ToString());
            msg.Content = buff;
        }

        public void ReadConfig()
        {
            
        }

        public static string EncodeURL(string url)
        {
            string ret = "";
            int index = 0;
            int i = 0;
            for (i = i; i < url.Length; i++)
            {
                char c = url[i];
                if (!Char.IsLetterOrDigit(c))
                {
                    byte asciiCode = (byte)c;
                    string str = asciiCode.ToString("X2");
                    ret += string.Format("%{0}", str);
                }
                else
                {
                    ret += c;
                }
            }
            return ret;
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
