namespace Transit.Business.Entities
{
    using System.Runtime.Serialization;

    using Core.Common.Contracts;
    using Core.Common.Core;

    [DataContract]
    public class Route : EntityBase, IIdentifiableEntity, IAccountOwnedEntity
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public int AuthorityId { get; set; }

        public int EntityId
        {
            get { return this.RouteId; }
            set { this.RouteId = value; }
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ShortName { get; set; }

        public int OwnerAccountId
        {
            get { return this.AccountId; }
        }

        [DataMember]
        public string Color { get; set; }

        [DataMember]
        public string TextColor { get; set; }

        [DataMember]
        public int RouteId { get; set; }

        [DataMember]
        public int Tag { get; set; }

        [DataMember]
        public int Type { get; set; }

        [DataMember]
        public string Url { get; set; }
    }
}