namespace Transit.Business.Entities
{
    using System.Runtime.Serialization;

    using Core.Common.Contracts;
    using Core.Common.Core;

    [DataContract]
    public class Stop : EntityBase, IIdentifiableEntity, IAccountOwnedEntity
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public int AuthorityId { get; set; }

        public int EntityId
        {
            get { return this.StopId; }
            set { this.StopId = value; }
        }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public string Name { get; set; }

        public int OwnerAccountId
        {
            get { return this.AccountId; }
        }

        [DataMember]
        public int StopId { get; set; }

        [DataMember]
        public int Tag { get; set; }

        [DataMember]
        public int Type { get; set; }
    }
}