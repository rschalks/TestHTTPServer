using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using LWHTTP;
using LWHTTP.HTTPCommon;
using LWHTTP.HTTPCommon.HTTPHeaders;
using System.Net;

namespace HTTPServerTest
{
    [TestClass]
    public class HTTPMessageTests
    {
        [TestMethod]
        public void ParseMimeTypeTest()
        {
            HTTPMIMEType a = HTTPMIMEType.Parse("text/html");
            Assert.AreEqual(a.Type, "text");
            Assert.AreEqual(a.Subtype, "html");
            HTTPMIMEType b = HTTPMIMEType.Parse("image/png;q=0.9");
            Assert.AreEqual(b.Type, "image");
            Assert.AreEqual(b.Subtype, "png");
            Assert.AreEqual(b.Params[0].Identifier, 'q');
            Assert.AreEqual(b.Params[0].Value, "0.9");
        }

        [TestMethod]
        public void RequestHeadersTest()
        {
            HTTPMessage msg = new HTTPMessage();
            msg.RequestMethod = HTTPRequestMethod.Get;
            msg.Version = HTTPVersion.HTTP11;
            msg.Path = "/";
            msg.Headers.AddRange(new HTTPHeader[]
            {
                new HTTPConnectionHeader()
                {
                    Content = HTTPConnection.KeepAlive
                },
                new HTTPHostHeader()
                {
                    Content = IPAddress.Parse("192.168.1.1")
                },
                new HTTPAcceptHeader()
                {
                    Content = new List<HTTPMIMEType>()
                    {
                        HTTPMIMEType.Parse("text/html"),
                        HTTPMIMEType.Parse("text/*"),
                        HTTPMIMEType.Parse("image/png;q=0.9"),
                    }
                },
            });

            //msg.ToHTTPString();
        }
    }
}
