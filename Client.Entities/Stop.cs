namespace Client.Entities
{
    using System;

    using Core.Common.Core;

    public class Stop : ObjectBase
    {
        private int authorityId;

        private double latitude;

        private double longitude;

        private string name;

        private int stopId;

        private string tag;

        private string type;

        public int AuthorityId
        {
            get { return this.authorityId; }
            set
            {
                if (this.authorityId != value)
                {
                    this.authorityId = value;
                    this.OnPropertyChanged(() => this.AuthorityId);
                }
            }
        }

        public double Latitude
        {
            get { return this.latitude; }
            set
            {
                if (Math.Abs(this.latitude - value) > 0.0001)
                {
                    this.latitude = value;
                    this.OnPropertyChanged(() => this.Latitude);
                }
            }
        }

        public double Longitude
        {
            get { return this.longitude; }
            set
            {
                if (Math.Abs(this.longitude - value) > 0.0001)
                {
                    this.longitude = value;
                    this.OnPropertyChanged(() => this.Longitude);
                }
            }
        }

        public string Name
        {
            get { return this.name; }
            set
            {
                if (this.name != value)
                {
                    this.name = value;
                    this.OnPropertyChanged(() => this.Name);
                }
            }
        }

        public int StopId
        {
            get { return this.stopId; }
            set
            {
                if (this.stopId != value)
                {
                    this.stopId = value;
                    this.OnPropertyChanged(() => this.StopId);
                }
            }
        }

        public string Tag
        {
            get { return this.tag; }
            set
            {
                if (this.tag != value)
                {
                    this.tag = value;
                    this.OnPropertyChanged(() => this.Tag);
                }
            }
        }

        public string Type
        {
            get { return this.type; }
            set
            {
                if (this.type != value)
                {
                    this.type = value;
                    this.OnPropertyChanged(() => this.Type);
                }
            }
        }
    }
}