namespace Client.Entities
{
    using Core.Common.Core;

    public class Account : ObjectBase
    {
        private int accountId;

        private string address;

        private string city;

        private string creditCard;

        private string expiryDate;

        private string firstName;

        private string lastName;

        private string loginEmail;

        private string state;

        private string zipCode;

        public int AccountId
        {
            get { return this.accountId; }
            set
            {
                if (this.accountId != value)
                {
                    this.accountId = value;
                    this.OnPropertyChanged(() => this.AccountId);
                }
            }
        }

        public string Address
        {
            get { return this.address; }
            set
            {
                if (this.address != value)
                {
                    this.address = value;
                    this.OnPropertyChanged(() => this.Address);
                }
            }
        }

        public string City
        {
            get { return this.city; }
            set
            {
                if (this.city != value)
                {
                    this.city = value;
                    this.OnPropertyChanged(() => this.City);
                }
            }
        }

        public string CreditCard
        {
            get { return this.creditCard; }
            set
            {
                if (this.creditCard != value)
                {
                    this.creditCard = value;
                    this.OnPropertyChanged(() => this.CreditCard);
                }
            }
        }

        public string ExpiryDate
        {
            get { return this.expiryDate; }
            set
            {
                if (this.expiryDate != value)
                {
                    this.expiryDate = value;
                    this.OnPropertyChanged(() => this.ExpiryDate);
                }
            }
        }

        public string FirstName
        {
            get { return this.firstName; }
            set
            {
                if (this.firstName != value)
                {
                    this.firstName = value;
                    this.OnPropertyChanged(() => this.FirstName);
                }
            }
        }

        public string LastName
        {
            get { return this.lastName; }
            set
            {
                if (this.lastName != value)
                {
                    this.lastName = value;
                    this.OnPropertyChanged(() => this.LastName);
                }
            }
        }

        public string LoginEmail
        {
            get { return this.loginEmail; }
            set
            {
                if (this.loginEmail != value)
                {
                    this.loginEmail = value;
                    this.OnPropertyChanged(() => this.LoginEmail);
                }
            }
        }

        public string State
        {
            get { return this.state; }
            set
            {
                if (this.state != value)
                {
                    this.state = value;
                    this.OnPropertyChanged(() => this.State);
                }
            }
        }

        public string ZipCode
        {
            get { return this.zipCode; }
            set
            {
                if (this.zipCode != value)
                {
                    this.zipCode = value;
                    this.OnPropertyChanged(() => this.ZipCode);
                }
            }
        }
    }
}