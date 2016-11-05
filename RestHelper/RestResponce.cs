using System.Collections.Generic;
using System.Net;

namespace RestHelper
{
    public class RestResponce
    {
        public HttpStatusCode StatusCode { get; set; }

        public int StatusNumber { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public virtual string Content { get; set; }
    }

    public class RestResponce<T> : RestResponce where T : new()
    {
        private string _content;

        public T Data { get; set; }

        public ISerializer Serializer { get; set; }

        public override string Content
        {
            get { return _content; }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.Data = Serializer.Deserialize<T>(value);
                }
                _content = value;
            }
        }

        public RestResponce(ISerializer serializer)
        {
            Serializer = serializer;
        }

        public RestResponce() : this(new SerializeToJson()) {}
    } 
}