using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTG_Durmaz.HTTP
{
    public class RequestContext
    {
        public HttpMethod Method { get; set; }
        public string ResourcePath { get; set; }
        public string HttpVersion { get; set; }
        public string token { get; set; }
        public Dictionary<string, string> Header { get; set; }
        public string? Payload { get; set; }

        public RequestContext()
        {
            Method = HttpMethod.Get;
            ResourcePath = "";
            token = "";
            HttpVersion = "HTTP/1.1";
            Header = new Dictionary<string, string>();
            Payload = null;
        }
    }
}
