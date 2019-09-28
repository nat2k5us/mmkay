namespace Sample.Models
{
    using System.Runtime.Serialization;

    [DataContract]
    public class FaultInfo
    {
        [DataMember]
        public string Reason;
    }
}