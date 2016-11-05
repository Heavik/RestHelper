using System.Net;

namespace RestHelper
{
    public class RestResponce
    {
        public HttpStatusCode StatusCode { get; set; }

        public string Content { get; set; }
    }
}