using System.IO;
using System.Xml.Serialization;

namespace Fred.Net.Utils
{
    public static class Deserializer
    {
        public static T Deserialize<T>(string xml)
        {
            StringReader reader = new StringReader(xml);

            XmlSerializer serializer = new XmlSerializer(typeof(T));

            T result = (T)serializer.Deserialize(reader);

            return result;
        }
    }
}