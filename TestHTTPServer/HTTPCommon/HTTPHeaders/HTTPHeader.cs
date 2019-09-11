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

        public override string ToString()
        {
            return string.Format("{0}: {1}", this.Title, HTTPHelpers.ConnectionTypeToString((HTTPConnection)this.Content));
        }
    }

    public class HTTPHostHeader : HTTPHeader
    {
        public int Port { get; set; }

        public HTTPHostHeader()
        {
            this.ContentType = typeof(string);
            this.Title = "Host";
            this.Port = -1;
        }

        public override void ParseContent(string str)
        {
            string hostStr = str;
            if (str.Contains(":"))
            {
                int colonIndex = str.IndexOf(':');
                string portStr = str.Substring(colonIndex + 1);
                Port = int.Parse(portStr);
            }
            this.Content = hostStr;
        }
    }

    public class HTTPAcceptHeader : HTTPHeader
    {
        public HTTPAcceptHeader()
        {
            this.ContentType = typeof(List<HTTPMIMEType>);
            this.Content = new List<HTTPMIMEType>();
            this.Title = "Accept";
        }

        public override void ParseContent(string str)
        {
            int nrOfCommas = str.Count(c => c == ',');
            int lastCommaIndex = 0;
            int commaIndex = 0;
            for (int i = 0; i < nrOfCommas+1; i++)
            {
                commaIndex = str.IndexOf(',', lastCommaIndex);
                int length = 0;
                if (commaIndex != -1)
                {
                    length = commaIndex - lastCommaIndex;
                }
                else
                {
                    length = str.Length - lastCommaIndex;
                }
                string substr = str.Substring(lastCommaIndex, length);
                HTTPMIMEType mimeType = HTTPMIMEType.Parse(substr);
                ((List<HTTPMIMEType>)this.Content).Add(mimeType);
                lastCommaIndex = commaIndex + 1;
            }
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            var list = ((List<HTTPMIMEType>)this.Content);
            sb.Append("Accept: ");
            for (int i = 0; i < list.Count; i++)
            {
                HTTPMIMEType typ = list[i];
                sb.Append(string.Format("{0}{1}", i == 0 ? "" : ",", typ.ToString()));
            }
            return sb.ToString();
        }
    }

    public class HTTPContentTypeHeader : HTTPHeader
    {
        public HTTPContentTypeHeader()
        {
            this.Title = "Content-Type";
            this.ContentType = typeof(HTTPMIMEType);
        }

        public override void ParseContent(string str)
        {
            this.Content = HTTPMIMEType.Parse(str);
        }
    }

    public class HTTPContentLengthHeader : HTTPHeader
    {
        public HTTPContentLengthHeader()
        {
            this.Title = "Content-Length";
            this.ContentType = typeof(int);
        }

        public override void ParseContent(string str)
        {
            this.Content = (object)int.Parse(str);
        }
    }
}
