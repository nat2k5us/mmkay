using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Xml.Serialization;

namespace PtcAdProcessor
{
    using System.Text;
    using System.Xml;

    public static class SerializationService
    {
        public static void SerializeToFile<T>(T obj, string fullPath) where T : class, new()
        {
            if (Directory.Exists(Path.GetDirectoryName(fullPath)))
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                using (var writer = new XmlTextWriter(stream, Encoding.Unicode))
                {
                    var xmlSerializer = new XmlSerializer(obj.GetType());
                    xmlSerializer.Serialize(writer, obj);
                }
            }
        }

        public static T DeserializeFromFile<T>(string fullPath) where T : class, new()
        {
            if (File.Exists(fullPath))
            {
                using (var stream = new FileStream(fullPath, FileMode.Open))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    return (T)xmlSerializer.Deserialize(stream);
                }
            }
            return default(T);
        }
    }

}