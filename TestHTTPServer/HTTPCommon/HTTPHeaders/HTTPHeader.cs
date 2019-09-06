using System;
using System.Collections.Generic;
using System.Text;
using LWHTTP.HTTPCommon;
using System.Reflection;
using System.Linq;
using System.Net;

namespace LWHTTP.HTTPCommon.HTTPHeaders
{
    /// <summary>
    /// This class contains information about a HTTP Header. Can not be created by itself.
    /// </summary>
    public class HTTPHeader
    {
        /// <summary>
        /// The title of this header.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The content of this header.
        /// </summary>
        public object Content { get; set; }
        /// <summary>
        /// The Type of the Content.
        /// </summary>
        public Type ContentType { get; protected set; }

        public HTTPHeader()
        {
            this.ContentType = typeof(String);
        }

        /// <summary>
        /// Returns this Header as an HTTP recognized string.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", this.Title, this.Content.ToString());
        }

        public virtual void ParseContent(string str)
        {
            this.Content = str;
        }

        public static HTTPHeader Parse(string str)
        {
            int colonIndex = str.IndexOf(':');
            string headerStr = str.Substring(0, colonIndex);
            string contentStr = str.Substring(colonIndex + 2);
            Console.WriteLine("Received header - {0}", str);
            HTTPHeader ret = null;
            // use reflection to get the correct header
            var headerType = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(t => t.GetTypes())
                .FirstOrDefault(t => (t.IsClass && t.Namespace == "LWHTTP.HTTPCommon.HTTPHeaders" && t.Name == String.Format("HTTP{0}Header", headerStr)));
            if (headerType != null)
            {
                ret = (HTTPHeader)Activator.CreateInstance(headerType);
                ret.ParseContent(contentStr);
                Console.WriteLine("Header is of known type {0}", headerType.FullName);
                return ret;
            }
            else
            {
                ret = new HTTPHeader();
                ret.Title = headerStr;
                ret.Content = contentStr;
            }

            return ret;
        }
    }
    
    public class HTTPConnectionHeader : HTTPHeader
    {
        public HTTPConnectionHeader()
        {
            this.ContentType = typeof(HTTPConnection);
            this.Title = "Connection";
        }

        public static string ConnectionTypeToString(HTTPConnection conn)
        {
            switch(conn)
            {
                case HTTPConnection.Close:
                    return "close";
                case HTTPConnection.KeepAlive:
                    return "keep-alive";
                case HTTPConnection.ProxyAuthenticate:
                    return "proxy-authenticate";
                case HTTPConnection.ProxyAuthorisation:
                    return "proxy-authorisation";
                case HTTPConnection.TE:
                    return "te";
                case HTTPConnection.Connection:
                    return "connection";
                case HTTPConnection.Trailer:
                    return "trailer";
                case HTTPConnection.TransferEncoding:
                    return "transfer-encoding";
                case HTTPConnection.Upgrade:
                    return "upgrade";
                default:
                    return "";
            }
        }

        public override void ParseContent(string str)
        {
            str = str.Replace("-", "").ToLower();
            string[] enumNames = Enum.GetNames(typeof(HTTPConnection));
            for (int i = 0; i < enumNames.Count(); i++)
            {
                if (enumNames[i].ToLower() == str)
                {
                    this.Content = (HTTPConnection)i;
                    return;
                }
            }
        }
    }

    public class HTTPHostHeader : HTTPHeader
    {
        public int Port { get; set; }

        public HTTPHostHeader()
        {
            this.ContentType = typeof(IPAddress);
            this.Title = "Host";
            this.Port = -1;
        }

        public override void ParseContent(string str)
        {
            string ipStr = str;
            if (str.Contains(":"))
            {
                int colonIndex = str.IndexOf(':');
                string portStr = str.Substring(colonIndex + 1);
                Port = int.Parse(portStr);
            }
            this.Content = IPAddress.Parse(ipStr);
        }
    }
}
