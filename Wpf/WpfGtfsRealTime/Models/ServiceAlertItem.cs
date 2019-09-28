// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceAlertItem.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime.Models
{
    using System;
    using System.Collections.Generic;

    using Prism.Mvvm;

    public class AffectedItem : BindableBase
    {
        private string route;
        public string Route
        {
            get
            {
                return this.route;
            }

            set
            {
                this.SetProperty(ref this.route, value);
            }
        }

        private string stopId;
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

        private string agency;
        public string Agency
        {
            get
            {
                return this.agency;
            }

            set
            {
                this.SetProperty(ref this.agency, value);
            }
        }


    }
    /// <summary>The service alert item.</summary>
    public class ServiceAlertItem : BindableBase
    {
        #region Fields

        /// <summary>The cause.</summary>
        private string cause;

        /// <summary>The description.</summary>
        private string description;

        /// <summary>The effect.</summary>
        private string effect;

        /// <summary>The end time.</summary>
        private DateTime endTime;

        /// <summary>The header.</summary>
        private string header;

        /// <summary>The start time.</summary>
        private DateTime startTime;

        /// <summary>The stops affected.</summary>
        private List<AffectedItem> stopsAffected;

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the cause.</summary>
        public string Cause
        {
            get
            {
                return this.cause;
            }

            set
            {
                this.SetProperty(ref this.cause, value);
            }
        }

      

        /// <summary>Gets or sets the description.</summary>
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.SetProperty(ref this.description, value);
            }
        }

        /// <summary>Gets or sets the effect.</summary>
        public string Effect
        {
            get
            {
                return this.effect;
            }

            set
            {
                this.SetProperty(ref this.effect, value);
            }
        }

        /// <summary>Gets or sets the end time.</summary>
        public DateTime EndTime
        {
            get
            {
                return this.endTime;
            }

            set
            {
                this.SetProperty(ref this.endTime, value);
            }
        }

        /// <summary>Gets or sets the header.</summary>
        public string Header
        {
            get
            {
                return this.header;
            }

            set
            {
                this.SetProperty(ref this.header, value);
            }
        }

        /// <summary>Gets or sets the start time.</summary>
        public DateTime StartTime
        {
            get
            {
                return this.startTime;
            }

            set
            {
                this.SetProperty(ref this.startTime, value);
            }
        }

        /// <summary>Gets or sets the stops affected.</summary>
        public List<AffectedItem> StopsAffected
        {
            get
            {
                return this.stopsAffected;
            }

            set
            {
                this.SetProperty(ref this.stopsAffected, value);
            }
        }

        #endregion
    }
}