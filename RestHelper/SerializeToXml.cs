using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RestHelper
{
    public class SerializeToXml: ISerializer
    {
        public string Serialize(object obj)
        {
            if (obj is IDictionary)
            {
                var document = MakeXmlDocument(obj as IDictionary<string, string>);
                return XmlToString(document);
            }
            var serializer = new XmlSerializer(obj.GetType());
            using (var writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }
       
        public string Merge(string obj, IDictionary<string, string> reqParams)
        {
            if (!reqParams.Any())
            {
                return obj;
            }
            var document1 = new XmlDocument();
            document1.LoadXml(obj);
            var document2 = MakeXmlDocument(reqParams);

            foreach (XmlNode node in document2.DocumentElement.ChildNodes)
            {
                var newNode = document1.ImportNode(node, true);
                document1.DocumentElement.AppendChild(newNode);
            }

            var str = XmlToString(document1);

            return str;
        }

        public T Deserialize<T>(string obj)
        {
            var serializer = new XmlSerializer(typeof(T));
            byte[] arr = Encoding.UTF8.GetBytes(obj);
            using (var xmlReader = XmlReader.Create(new MemoryStream(arr)))
            {
                return (T)serializer.Deserialize(xmlReader);
            }
        }

        private XmlDocument MakeXmlDocument(IDictionary<string, string> reqParams)
        {
            var document = new XmlDocument();
            var declaration = document.CreateXmlDeclaration("1.0", "UTF-8", null);            
            document.InsertBefore(declaration, document.DocumentElement);
            var root = document.CreateElement(string.Empty, "xml", string.Empty);
            document.AppendChild(root);

            foreach (var reqParam in reqParams)
            {
                var element = document.CreateElement(string.Empty, reqParam.Key, string.Empty);
                var text = document.CreateTextNode(reqParam.Value);
                element.AppendChild(text);
                root.AppendChild(element);
            }

            return document;
        }

        private string XmlToString(XmlDocument document)
        {
            using (var stringWriter = new StringWriter())
            {
                using (var xmlWriter = XmlWriter.Create(stringWriter))
                {
                    document.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                    return stringWriter.GetStringBuilder().ToString();
                }
            }
        }
    }
}