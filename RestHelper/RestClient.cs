using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace RestHelper
{
    public class RestClient
    {
        private WebRequest _webRequest;

        private readonly string _domain;

        private readonly Dictionary<Method, string> _methods = new Dictionary<Method, string>()
        {
            {Method.GET, "GET"},
            {Method.POST, "POST"},
            {Method.PUT, "PUT"},
            {Method.DELETE, "DELETE"}
        };

        public RestClient(string domain)
        {
            this._domain = domain;
        }

        public RestResponce Execute(RestRequest request)
        {
            InitRequest(request);
            SetMethod(request);
            SetHeaders(request);
            if (request.Method == Method.POST || request.Method == Method.PUT)
            {
                SetRequestBody(request);
            }
            return GetResponce();           
        }

        private void InitRequest(RestRequest request)
        {
            string relativePath = request.UrlTemplate;
            if (request.Method == Method.GET || request.Method == Method.DELETE)
            {
                relativePath = SetQueryString(relativePath, request.Params);
            }
            string url = _domain + "/" + relativePath;
            _webRequest = WebRequest.Create(url);
        }

        private RestResponce GetResponce()
        {
            var responce = new RestResponce();
            try
            {
                var webResponse = _webRequest.GetResponse() as HttpWebResponse;
                if (webResponse != null)
                {
                    using (var reader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        responce.StatusCode = webResponse.StatusCode;                       
                        responce.StatusNumber = Convert.ToInt32(webResponse.StatusCode);
                        responce.Headers = GetResponceHeaders(webResponse);
                        responce.Content = reader.ReadToEnd();
                    }
                    webResponse.Close();
                }
            }
            catch (WebException ex)
            {
                var resp = ex.Response as HttpWebResponse;
                responce.StatusCode = resp.StatusCode;
                responce.StatusNumber = Convert.ToInt32(resp.StatusCode);
                responce.Headers = GetResponceHeaders(resp);
                responce.Content = string.Empty;
                resp.Close();
            }
            
            return responce;
        }

        private void SetMethod(RestRequest request)
        {
            _webRequest.Method = _methods[request.Method];
        }

        private void SetHeaders(RestRequest request)
        {
            foreach (var header in request.Headers)
            {
                _webRequest.Headers.Add(header.Key, header.Value);
            }
        }

        private string SetQueryString(string path, string query)
        {
            return string.Format("{0}?{1}", path, query);
        }

        private void SetRequestBody(RestRequest request)
        {
            byte[] data = Encoding.UTF8.GetBytes(request.Params);
            _webRequest.ContentLength = data.Length;
            using (var requestStream = _webRequest.GetRequestStream())
            {               
                requestStream.Write(data, 0, data.Length);
            }
        }

        private Dictionary<string, string> GetResponceHeaders(HttpWebResponse response)
        {
            var headers = new Dictionary<string, string>();
            foreach(var header in response.Headers.Keys)
            {
                headers.Add(header.ToString(), response.Headers[header.ToString()]);
            }

            return headers;
        }
    }
}