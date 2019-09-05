using System;
using System.Collections.Generic;
using System.Text;
using LWHTTP.HTTPCommon;

namespace LWHTTP.HTTPCommon.HTTPHeaders
{
    /// <summary>
    /// This interface exists to allow you to put generic items into a generic IENumerable.
    /// </summary>
    public interface IHTTPHeader
    {
        Type ContentType { get; }
    }

    /// <summary>
    /// This class contains information about a HTTP Header.
    /// </summary>
    /// <typeparam name="TFormat">Generic type, where TFormat must be an instance of IFormattable.</typeparam>
    public abstract class HTTPHeader<TFormat> : IHTTPHeader where TFormat : IFormattable
    {
        /// <summary>
        /// The title of this header.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// The content of this header.
        /// </summary>
        public TFormat Content { get; set; }
        /// <summary>
        /// The Type of the Content.
        /// </summary>
        public Type ContentType { get { return typeof(TFormat); } }

        /// <summary>
        /// Returns this Header as an HTTP recognized string.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString()
        {
            return String.Format("{0}: {1}", this.Title, this.Content.ToString());
        }

        /// <summary>
        /// Returns this Header as an HTTP recognized string. 
        /// </summary>
        /// <param name="format">The format to pass onto the content ToString</param>
        /// <param name="formatProvider">The FormatProvider to pass onto the content ToString</param>
        /// <returns>string</returns>
        public virtual string ToString(string format, IFormatProvider formatProvider)
        {
            return string.Format("{0}: {1}", this.Title, this.Content.ToString(format, formatProvider));
        }
    }
    
    public class HTTPConnectionHeader : HTTPHeader<HTTPConnection>
    {
        HTTPConnection ConnectionType { get; set; }

        public HTTPConnectionHeader()
        {
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
    }
}
