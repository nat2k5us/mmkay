// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServiceAlertsViewModel.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using System.Windows.Input;

    //using Luminator.Metro.Common;
    //using Luminator.Metro.Types;
    //using Luminator.TransitProvider.GtfsRealTime;
    //using Luminator.TransitProvider.Models;
    //using Luminator.TransitProvider.Settings;
    //using Luminator.Utility;

    using Prism.Commands;
    using Prism.Mvvm;

    using WpfGtfsRealTime.Models;
    using WpfGtfsRealTime.ServiceCalls;

    using Timer = System.Timers.Timer;

    /// <summary>The service alerts view model.</summary>
    public class ServiceAlertsViewModel : BindableBase
    {
        #region Fields

        /// <summary>The timer.</summary>
        private Timer timerServiceAlerts;

        /// <summary>The selected service alert.</summary>
        private ServiceAlertItem selectedServiceAlert;

        /// <summary>The schedule prediction item collection.</summary>
        private ObservableCollection<ServiceAlertItem> serviceAlertItems;


        private ICollectionView serviceAlertsView { get; set; }

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="ServiceAlertsViewModel"/> class.</summary>
        public ServiceAlertsViewModel()
        {
            this.ServiceAlertItems = new ObservableCollection<ServiceAlertItem>();

            this.ServiceAlertSelectedCommand = new DelegateCommand(this.ServiceAlertSelected, this.CanSelectServiceAlerts).ObservesProperty(() => this.ServiceAlertItems);
            this.ServiceAlertsGetCommand = new DelegateCommand(this.GetServiceAlerts, this.CanGetServiceAlerts).ObservesProperty(() => this.ServiceAlertItems);
            this.RouteFilterTextChangedCommand = new DelegateCommand(this.FilterRoutes, this.CanFilterRoutes).ObservesProperty(() => this.ServiceAlertItems);
            this.UIContext = SynchronizationContext.Current;
        }

        private bool CanFilterRoutes()
        {
           return true;
        }

        private void FilterRoutes()
        {
            this.UIContext.Send(
                update =>
                    {
                        ObservableCollection<ServiceAlertItem> filteredAlertItems = new ObservableCollection<ServiceAlertItem>();
                        foreach (var serviceAlertItem in this.ServiceAlertItems)
                        {
                            foreach (var affectedItem in serviceAlertItem.StopsAffected)
                            {
                                var filter = this.RouteFilter;
                                if (filter != null)
                                {
                                    foreach (var filterItem in filter.Split(',').ToList())
                                    {
                                        if (filterItem == affectedItem.Route) filteredAlertItems.Add(serviceAlertItem);
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }

                        this.ServiceAlertItems = filteredAlertItems;
                    },
                null);
        }

        #endregion

        #region Public Properties

        /// <summary>Gets or sets the selected service alert.</summary>
        public ServiceAlertItem SelectedServiceAlert
        {
            get
            {
                return this.selectedServiceAlert;
            }

            set
            {
                this.SetProperty(ref this.selectedServiceAlert, value);
            }
        }

        private bool enabledRoutesOnly;
        public bool EnabledRoutesOnly
        {
            get
            {
                return this.enabledRoutesOnly;
            }

            set
            {
                this.SetProperty(ref this.enabledRoutesOnly, value);
            }
        }
        private string errorStatus;
        public string ErrorStatus
        {
            get
            {
                return this.errorStatus;
            }

            set
            {
                this.SetProperty(ref this.errorStatus, value);
            }
        }
        private string routeFilter;
        public string RouteFilter
        {
            get
            {
                return this.routeFilter;
            }

            set
            {
                this.SetProperty(ref this.routeFilter, value);
            }
        }
        

        /// <summary>Gets or sets the service alert items.</summary>
        public ObservableCollection<ServiceAlertItem> ServiceAlertItems
        {
            get
            {
                return this.serviceAlertItems;
            }

            set
            {
                this.SetProperty(ref this.serviceAlertItems, value);
            }
        }

        /// <summary>Gets or sets the predictions command.</summary>
        public ICommand ServiceAlertSelectedCommand { get; set; }

        /// <summary>Gets or sets the service alerts get command.</summary>
        public ICommand ServiceAlertsGetCommand { get; set; }

        public ICommand RouteFilterTextChangedCommand { get; set; }

        /// <summary>Gets the ui context.</summary>
        public SynchronizationContext UIContext { get; }

        #endregion

        #region Methods

        /// <summary>The can get service alerts.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CanGetServiceAlerts()
        {
            return this.ServiceAlertItems != null;
        }

        /// <summary>The can select service alerts.</summary>
        /// <returns>The <see cref="bool"/>.</returns>
        /// <exception cref="NotImplementedException"></exception>
        private bool CanSelectServiceAlerts()
        {
            return this.ServiceAlertItems != null;
        }
        private object runningTask;
        /// <summary>The get service alerts.</summary>
        /// <exception cref="NotImplementedException"></exception>
        private void GetServiceAlerts()
        {
            var timeZone = UrlLocationsUtil.Instance.GetTimeZone(TransitProviderType.GtfsRealTime, Constants.CalgaryJurisdiction);
            var inTimeZone = DateTime.Now.ConvertToTimeZone(timeZone);
            Debug.WriteLine(inTimeZone);
            var alertItems = new List<ServiceAlertItem>();
            var sw = new Stopwatch();
            sw.Start();
            runningTask = Task.Run(
                () =>
                    {
                        var alertsFeed = GtfsRealTimeServiceAlerts.GetServiceAlerts(Constants.CalgaryJurisdiction);
                        foreach (var feedEntity in alertsFeed.entity)
                        {
                            var timeRange = feedEntity.alert.active_period.FirstOrDefault();

                            if (timeRange != null)
                            {
                               
                                var stopsAffected = (from x in feedEntity.alert.informed_entity
                                                     where this.EnabledRoutesOnly == false || (VmbProviderAccess.IsVmbAvailable() && VmbProviderAccess.IsRouteValid(x.route_id, "Calgary") == true)
                                                     select new AffectedItem { Agency = x.agency_id, Route = x.route_id, StopId = x.stop_id}).ToList();
                                if (!stopsAffected.Any()) continue;
                                var serviceAlert = new ServiceAlertItem
                                                       {
                                                           Cause = feedEntity.alert.cause.ToString(),
                                                           Description = feedEntity.alert.description_text.translation.First().text,
                                                           Effect = feedEntity.alert.effect.ToString(),
                                                           Header = feedEntity.alert.header_text.translation.First().text,
                                                           StopsAffected = stopsAffected,
                                                           StartTime = timeRange.start.ConvertFromEpochTime(timeZone),
                                                           EndTime = timeRange.end.ConvertFromEpochTime(timeZone),

                                                       };
                                alertItems.Add(serviceAlert);
                            }
                        }
                    }).ContinueWith(
                        previousTask =>
                            {
                                this.UIContext.Send(
                                    update =>
                                        {
                                            this.ErrorStatus = alertItems.Count == 0 ? (this.EnabledRoutesOnly ? "No Service Alerts Found - Make sure Services are running": "No Alerts found") : $" {alertItems.Count} Alerts Found";
                                            this.ServiceAlertItems = new ObservableCollection<ServiceAlertItem>(alertItems);
                                        },
                                    null);
                            });
        }

        /// <summary>The service alert selected.</summary>
        /// <exception cref="NotImplementedException"></exception>
        private void ServiceAlertSelected()
        {
           
        }

        #endregion
    }
}