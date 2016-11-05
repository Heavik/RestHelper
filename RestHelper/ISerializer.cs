using System.Collections.Generic;

namespace RestHelper
{
    public interface ISerializer
    {
        string Serialize(object obj);

        string Merge(string obj, IDictionary<string, string> reqParams);

        T Deserialize<T>(string obj);
    }
}