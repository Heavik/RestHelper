using System.Collections.Generic;
using System.Net;

namespace RestHelper
{
    public class RestResponce
    {
        public HttpStatusCode StatusCode { get; set; }

        public int StatusNumber { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public string Content { get; set; }
    }
}