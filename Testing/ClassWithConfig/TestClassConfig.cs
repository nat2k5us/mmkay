namespace ClassWithConfig
{
    using System.Xml;
    using System.Xml.Linq;

    using ExtendedXmlSerialization;

    public class TestClassConfig : ExtendedXmlSerializerConfig<TestClass>
    {
        public TestClassConfig()
        {
            this.CustomSerializer(this.Serializer, this.Deserialize);
        }

        public TestClass Deserialize(XElement element)
        {
            return new TestClass(element.Element("String").Value);
        }

        public void Serializer(XmlWriter writer, TestClass obj)
        {
            writer.WriteElementString("String", obj.PropStr);
        }
    }
}