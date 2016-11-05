using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace RestHelper
{
    public class RestRequest
    {
        private string _urlTemplate;

        private string _requestObject;

        private Serialize _serializationType;

        private readonly Dictionary<string, string> _bodyParams = new Dictionary<string, string>();

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();

        public Method Method { get; set; }

        public string UrlTemplate { get { return _urlTemplate; } }

        public string RequestObject { get { return _requestObject; } }

        public string Params
        {
            get
            {
                string reqParams = string.Empty;
                if (_bodyParams.Any())
                {
                    reqParams = _bodyParams
                     .Select(item => string.Format("{0}={1}", item.Key, item.Value))
                     .Aggregate((pair1, pair2) => string.Format("{0}&{1}", pair1, pair2));
                }
                return reqParams;
            }
        }

        public Dictionary<string, string> Headers { get { return _headers; } }

        public RestRequest(string urlTemplate, Method method, Serialize serializationType)
        {
            this._urlTemplate = urlTemplate;
            this.Method = method;
            this._serializationType = serializationType;
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
            switch (_serializationType)
            {
                case Serialize.Xml:
                    _requestObject = ToXml(obj);
                    break;
                case Serialize.Json:
                    _requestObject = ToJson(obj);
                    break;
            }
        }

        private string ToXml(object obj)
        {          
            var serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        private string ToJson(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}