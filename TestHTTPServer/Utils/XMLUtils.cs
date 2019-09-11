using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Reflection;

namespace LWHTTP.Utils.XML
{
    public class XMLConfig
    {
        public void Initialize()
        {
            string filePath = Assembly.GetExecutingAssembly().Location;
            filePath = filePath.Substring(0, filePath.LastIndexOf('\\'));
            if (!File.Exists(filePath + "\\conf.xml"))
            {
                XmlDocument doc = new XmlDocument();
            }
        }
    }
}
