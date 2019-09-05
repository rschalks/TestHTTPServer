using System;
using System.Collections.Generic;
using LWHTTP.HTTPCommon.HTTPHeaders;
using System.Text;

namespace LWHTTP.HTTPCommon
{
    public class HTTPMessage
    {
        /// <summary>
        /// A list of Headers this Message contains.
        /// </summary>
        public List<IHTTPHeader> Headers { get; set; }
        /// <summary>
        /// The RequestMethod of a Request. Becomes irrelevant when this message is a Response.
        /// </summary>
        public HTTPRequestMethod RequestMethod { get; set; }
        /// <summary>
        /// Determines if this Message is a Response. If it is not a Response (true), it is a Request (false).
        /// </summary>
        public bool IsResponse { get; set; }
        /// <summary>
        /// The HTTP version of this message.
        /// </summary>
        public HTTPVersion Version { get; set; }

        public HTTPMessage()
        {
            this.Headers = new List<IHTTPHeader>();
            this.Version = HTTPVersion.None;
        }
    }
}
