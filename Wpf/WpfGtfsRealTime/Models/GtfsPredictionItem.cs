namespace WpfGtfsRealTime.Models
{
    using System;

    using Prism.Mvvm;

    public class GtfsPredictionItem : BindableBase
    {
        #region Fields

        private int etaInMin;

        private string routeName;

        private string stopId;

        private string stopName;

        private bool timeUpdated;

        private string tripId;

        private DateTime realTimeEtaTime;

        #endregion

        #region Public Properties

        public int EtaInMin
        {
            get
            {
                return this.etaInMin;
            }

            set
            {
                this.SetProperty(ref this.etaInMin, value);
            }
        }

        public DateTime RealTimeEtaTime
        {
            get
            {
                return this.realTimeEtaTime;
            }

            set
            {
                this.SetProperty(ref this.realTimeEtaTime, value);
            }
        }

        private DateTime scheduledEtaTime;
        public DateTime ScheduledEtaTime
        {
            get
            {
                return this.scheduledEtaTime;
            }

            set
            {
                this.SetProperty(ref this.scheduledEtaTime, value);
            }
        }

        public string RouteName
        {
            get
            {
                return this.routeName;
            }

            set
            {
                this.SetProperty(ref this.routeName, value);
            }
        }

        public string StopId
        {
            get
            {
                return this.stopId;
            }

            set
            {
                this.SetProperty(ref this.stopId, value);
            }
        }

        public string StopName
        {
            get
            {
                return this.stopName;
            }

            set
            {
                this.SetProperty(ref this.stopName, value);
            }
        }

        public bool TimeUpdated
        {
            get
            {
                return this.timeUpdated;
            }

            set
            {
                this.SetProperty(ref this.timeUpdated, value);
            }
        }

        public string TripId
        {
            get
            {
                return this.tripId;
            }

            set
            {
                this.SetProperty(ref this.tripId, value);
            }
        }

        private string routeId;
        public string RouteId
        {
            get
            {
                return this.routeId;
            }

            set
            {
                this.SetProperty(ref this.routeId, value);
            }
        }

        private int etaInMinNonAbs;
        public int EtaInMinNonAbs
        {
            get
            {
                return this.etaInMinNonAbs;
            }

            set
            {
                this.SetProperty(ref this.etaInMinNonAbs, value);
            }
        }

        private int direction;
        public int Direction
        {
            get
            {
                return this.direction;
            }

            set
            {
                this.SetProperty(ref this.direction, value);
            }
        }

        private string arrivalRawText;
        public string ArrivalRawText
        {
            get
            {
                return this.arrivalRawText;
            }

            set
            {
                this.SetProperty(ref this.arrivalRawText, value);
            }
        }
        #endregion

        #region Public Methods and Operators

        public override string ToString()
        {
            return
                $"{nameof(this.StopId)} : {this.StopId}, {nameof(this.EtaInMin)} : {this.EtaInMin}, {nameof(this.RealTimeEtaTime)} : {this.RealTimeEtaTime} ,{nameof(this.StopName)} : {this.StopName},  {nameof(this.RouteName)} : {this.RouteName},  {nameof(this.TripId)} : {this.TripId} ";
        }

        #endregion

        // private int stopSequence;
    }
}