using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassWithConfig
{
    using System.Xml;
    using System.Xml.Linq;

    using ExtendedXmlSerialization;

    using Utilities;

    class Program
    {
        static void Main(string[] args)
        {

            var toolsFactory = new SimpleSerializationToolsFactory();
            toolsFactory.Configurations.Add(new TestClassConfig());
            ExtendedXmlSerializer serializer = new ExtendedXmlSerializer(toolsFactory);
     
            Console.WriteLine("Serialization");
            var obj = new TestClass("Natraj Bontha");
            var xml = serializer.Serialize(obj);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.Save(obj.GetType().Name.AddFileExtension("xml"));

            Console.WriteLine(xml);

            Console.WriteLine("Deserialization");
            var obj2 = serializer.Deserialize<TestClass>(xml);
            Console.WriteLine("Obiect id = " + obj2.PropStr);

            Console.ReadKey();
        }
    }
}
