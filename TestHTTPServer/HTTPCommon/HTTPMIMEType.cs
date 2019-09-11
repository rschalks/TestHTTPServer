using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LWHTTP.HTTPCommon
{
    public class HTTPMimeTypeParam
    {
        public char Identifier { get; set; }
        public object Value { get; set; }
    }

    public class HTTPMIMEType
    {
        public string Type { get; set; }
        public string Subtype { get; set; } 
        public List<HTTPMimeTypeParam> Params { get; set; }

        public HTTPMIMEType()
        {
            this.Params = new List<HTTPMimeTypeParam>();
        }

        public static HTTPMIMEType Parse(string str)
        {
            HTTPMIMEType typ = new HTTPMIMEType();
            int charIndex = str.IndexOf("/");
            typ.Type = str.Substring(0, charIndex);
            charIndex += 1;
            int semiColonCount = str.Count(c => c == ';');
            int length = 0;
            if (semiColonCount == 0)
            {
                length = str.Length - charIndex;
            }
            else
            {
                length = str.IndexOf(';') - charIndex;
            }
            typ.Subtype = str.Substring(charIndex, length);

            int lastCharIndex = str.IndexOf(';');
            for (int i = 0; i < semiColonCount; i++)
            {
                charIndex = str.IndexOf(';', lastCharIndex+1);
                if (charIndex != -1)
                {
                    length = charIndex - lastCharIndex;
                }
                else
                {
                    length = str.Length - lastCharIndex;
                }

                string substr = str.Substring(lastCharIndex, length);
                HTTPMimeTypeParam param = new HTTPMimeTypeParam();
                param.Identifier = substr[1];
                switch (param.Identifier)
                {
                    default:
                        param.Value = substr.Substring(3);
                        break;
                }
                typ.Params.Add(param);
            }

            return typ;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}/{1}", this.Type, this.Subtype));
            for (int i = 0; i < Params.Count; i++)
            {
                HTTPMimeTypeParam param = Params[i];
                sb.Append(string.Format(";{0}={1}", param.Identifier, param.Value.ToString()));
            }
            return sb.ToString();
        }
    }
}
