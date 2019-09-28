namespace Transit.Business.Entities
{
    using System.Runtime.Serialization;

    using Core.Common.Contracts;
    using Core.Common.Core;

    [DataContract]
    public class Account : EntityBase, IIdentifiableEntity, IAccountOwnedEntity
    {
        [DataMember]
        public int AccountId { get; set; }

        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string CreditCard { get; set; }

        public int EntityId
        {
            get { return this.AccountId; }
            set { this.AccountId = value; }
        }

        [DataMember]
        public string ExpDate { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string LastName { get; set; }

        [DataMember]
        public string LoginEmail { get; set; }

        public int OwnerAccountId
        {
            get { return this.AccountId; }
        }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string ZipCode { get; set; }
    }
}