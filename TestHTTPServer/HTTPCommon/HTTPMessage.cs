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
        public List<HTTPHeader> Headers { get; set; }
        /// <summary>
        /// The RequestMethod of a Request. Becomes irrelevant when this message is a Response.
        /// </summary>
        public HTTPRequestMethod RequestMethod { get; set; }
        /// <summary>
        /// Determines if this Message is a Response. If it is not a Response (true), it is a Request (false).
        /// </summary>
        public bool IsResponse { get; set; }
        /// <summary>
        /// The StatusCode of this Message. Becomes irrelevant if it is a Request, as StatusCodes are only for Responses.
        /// </summary>
        public HTTPStatusCode StatusCode { get; set; }
        /// <summary>
        /// The HTTP version of this message.
        /// </summary>
        public HTTPVersion Version { get; set; }

        public HTTPMessage()
        {
            this.Headers = new List<HTTPHeader>();
            this.Version = HTTPVersion.None;
        }

        public static HTTPMessage Parse(string data)
        {
            HTTPMessage ret = new HTTPMessage();

            int slashIndex = data.IndexOf('/');
            int spaceIndex = data.IndexOf(' ');

            string requestMethodStr = data.Substring(0, data.IndexOf('/')-1);
            string[] requestMethods = Enum.GetNames(typeof(HTTPRequestMethod));
            HTTPRequestMethod method = HTTPRequestMethod.Connect;
            for (int i = 0; i < requestMethods.Length; i++)
            {
                requestMethods[i] = requestMethods[i].ToUpper();
                if (requestMethods[i] == requestMethodStr)
                {
                    ret.RequestMethod = (HTTPRequestMethod)i;
                    Console.WriteLine("Received a {0} request.", requestMethodStr);
                    break;
                }
            }

            spaceIndex = data.IndexOf(' ', spaceIndex + 1);
            string path = data.Substring(slashIndex, spaceIndex - slashIndex);
            Console.WriteLine("Path: {0}", path);

            string httpVersionStr = data.Substring(spaceIndex + path.Length, 8);
            Console.WriteLine("HTTP Version: {0}", httpVersionStr);

            string[] httpVersionStrs = Enum.GetNames(typeof(HTTPVersion));
            for (int i = 0;  i < httpVersionStrs.Length; i++)
            {
                string str = httpVersionStr.Replace(".", "").Replace("/","");
                if (httpVersionStrs[i] == str)
                {
                    ret.Version = (HTTPVersion)i;
                }
            }
            if (ret.Version == HTTPVersion.None || ret.Version < Program.Server.SupportedVersion)
            {
                Console.WriteLine("Version {0} is not supported - the server uses version {1}.", httpVersionStr, Program.Server.SupportedVersion.ToString());
                ret.Version = HTTPVersion.Unsupported;
            }

            bool loopingHeaders = true;
            string[] lines = data.Split(new[] { "\n" }, StringSplitOptions.None);
            int lineIndex = 1;
            while (loopingHeaders)
            {
                string line = lines[lineIndex].Replace("\r","");
                if (line == "")
                {
                    loopingHeaders = false;
                    break;
                }
                HTTPHeader header = HTTPHeader.Parse(line);
                ret.Headers.Add(header);
                lineIndex += 1;
            }
            return ret;
        }
    }
}
