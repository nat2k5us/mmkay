// AdaptorNetworkInfo.cs

namespace NetoworkLib
{
    using System;
    using System.IO;
    using System.Net;
    using System.Reflection;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class AdaptorNetworkInfo
    {
        public string Name { get; set; }

        public bool IsDhcpEnabled { get; set; }

        [XmlIgnore]
        public IPAddress IpAddress { get; set; }

        public IPAddress Gateway { get; set; }

        public string Description { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                sb.Append(property.Name);
                sb.Append(": ");
                if (property.GetIndexParameters().Length > 0)
                {
                    sb.Append("Indexed Property cannot be used");
                }
                else
                {
                    sb.Append(property.GetValue(this, null));
                }

                sb.Append(Environment.NewLine);
            }

            return sb.ToString();
        }

        public string ToXml(object obj, Type objType)
        {
            try
            {
                XmlSerializerNamespaces serializeObject = new XmlSerializerNamespaces();
                var xmlSerializer = new XmlSerializer((objType));
                var memStream = new MemoryStream();
                var xmlWriter = new XmlTextWriter(memStream, Encoding.UTF8) { Namespaces = true };
                //XmlQualifiedName[] qualiArrayXML = serializeObject.ToArray();
                xmlSerializer.Serialize(xmlWriter, obj);
                xmlWriter.Close();
                memStream.Close();
                string xml;
                xml = Encoding.UTF8.GetString(memStream.GetBuffer());
                xml = xml.Substring(xml.IndexOf(Convert.ToChar(60)));
                xml = xml.Substring(0, (xml.LastIndexOf(Convert.ToChar(62)) + 1));
                return xml;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + (ex.InnerException?.Message ?? "No Inner exception"));
                return string.Empty;
            }
        }
    }
}