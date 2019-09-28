namespace Client.Entities
{
    using Core.Common.Core;

    public class Route : ObjectBase
    {
        private int authorityId;

        private string routeColor;

        private int routeId;

        private string routeLongName;

        private string routeName;

        private string routeShortName;

        private string routeTextColor;

        private string routeType;

        private string url;

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

        public string RouteColor
        {
            get { return this.routeColor; }
            set
            {
                if (this.routeColor != value)
                {
                    this.routeColor = value;
                    this.OnPropertyChanged(() => this.RouteColor);
                }
            }
        }

        public int RouteId
        {
            get { return this.routeId; }
            set
            {
                if (this.routeId != value)
                {
                    this.routeId = value;
                    this.OnPropertyChanged(() => this.RouteId);
                }
            }
        }

        public string RouteLongName
        {
            get { return this.routeLongName; }
            set
            {
                if (this.routeLongName != value)
                {
                    this.routeLongName = value;
                    this.OnPropertyChanged(() => this.RouteLongName);
                }
            }
        }

        public string RouteName
        {
            get { return this.routeName; }
            set
            {
                if (this.routeName != value)
                {
                    this.routeName = value;
                    this.OnPropertyChanged(() => this.RouteName);
                }
            }
        }

        public string RouteShortName
        {
            get { return this.routeShortName; }
            set
            {
                if (this.routeShortName != value)
                {
                    this.routeShortName = value;
                    this.OnPropertyChanged(() => this.RouteShortName);
                }
            }
        }

        public string RouteTextColor
        {
            get { return this.routeTextColor; }
            set
            {
                if (this.routeTextColor != value)
                {
                    this.routeTextColor = value;
                    this.OnPropertyChanged(() => this.RouteTextColor);
                }
            }
        }

        public string RouteType
        {
            get { return this.routeType; }
            set
            {
                if (this.routeType != value)
                {
                    this.routeType = value;
                    this.OnPropertyChanged(() => this.RouteType);
                }
            }
        }

        public string Url
        {
            get { return this.url; }
            set
            {
                if (this.url != value)
                {
                    this.url = value;
                    this.OnPropertyChanged(() => this.Url);
                }
            }
        }
    }
}