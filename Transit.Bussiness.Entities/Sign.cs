namespace Transit.Business.Entities
{
    using System.Runtime.Serialization;

    using Core.Common.Contracts;
    using Core.Common.Core;

    [DataContract]
    public class Sign : EntityBase, IIdentifiableEntity, IAccountOwnedEntity
    {
        [DataMember]
        public int AccountId { get; set; }

        public int EntityId
        {
            get { return this.SignId; }
            set { this.SignId = value; }
        }

        public int OwnerAccountId
        {
            get { return this.AccountId; }
        }

        [DataMember]
        public int SignId { get; set; }
    }
}