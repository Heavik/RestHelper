using System.Collections.Generic;
using System.Linq;

namespace RestHelper
{
    public class RestRequest
    {
        private string _urlTemplate;

        private string _requestObject;

        private readonly Dictionary<string, string> _bodyParams = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public ISerializer Serializer { get; set; }

        public Method Method { get; set; }

        public string UrlTemplate { get { return _urlTemplate; } }

        public string RequestObject { get { return _requestObject; } }

        public string Params
        {
            get
            {
                string reqParams = string.Empty;
                if (Method == Method.GET || Method == Method.DELETE || Serializer == null)
                {
                    if (_bodyParams.Any())
                    {
                        reqParams = _bodyParams
                            .Select(item => string.Format("{0}={1}", item.Key, item.Value))
                            .Aggregate((pair1, pair2) => string.Format("{0}&{1}", pair1, pair2));
                    }
                }
                else
                {
                    reqParams = string.IsNullOrEmpty(_requestObject) 
                        ? Serializer.Serialize(_bodyParams) 
                        : Serializer.Merge(_requestObject, _bodyParams);
                }
               
                return reqParams;
            }
        }

        public Dictionary<string, string> Headers { get { return _headers; } }

        public RestRequest(string urlTemplate, Method method, Serialize serializationType)
        {
            this._urlTemplate = urlTemplate;
            this.Method = method;        
            switch (serializationType)
            {
                case Serialize.Xml:
                    Serializer = new SerializeToXml();
                    break;
                case Serialize.Json:
                    Serializer = new SerializeToJson();
                    break;
            }
        }

        public RestRequest(string urlTemplate, Method method) : this(urlTemplate, method, Serialize.None) {}

        public RestRequest(string urlTemplate) : this(urlTemplate, Method.GET) {}

        public void AddParameter(string name, string value)
        {
            _bodyParams.Add(name, value);
        }

        public void AddHeader(string name, string value)
        {
            _headers.Add(name, value);
        }

        public void AddUrlSegment(string name, string value)
        {
            _urlTemplate = _urlTemplate.Replace("{" + name + "}", value);
        }

        public void AddObject(object obj)
        {
            if (Serializer != null)
            {
                _requestObject = Serializer.Serialize(obj);
            }
        }
    }
}