using System;
using System.Collections.Generic;
using System.Text;

namespace TestHTTPServer.HTTPCommon
{
    class HTTPMIMEType
    {
        String Type { get; set; }
        string Subtype { get; set; } 
        float Quality { get; set; }

        public HTTPMIMEType()
        {
            this.Quality = -1;
        }

        public static HTTPMIMEType Parse(string str)
        {
            HTTPMIMEType typ = new HTTPMIMEType();
            return null;
        }
    }
}
