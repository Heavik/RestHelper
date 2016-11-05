using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RestHelper
{
    public class SerializeToJson : ISerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }

        public string Merge(string obj, IDictionary<string, string> reqParams)
        {
            if (!reqParams.Any())
            {
                return obj;
            }
            var json = JObject.Parse(obj);
            foreach (var param in reqParams)
            {
                json.Add(param.Key, param.Value);
            }
            return json.ToString();
        }
    }
}