// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PredictionWindowViewModel.cs" company="Luminator">
//   Copyright (c) 2017
//   Luminator Technology Group
//   All Rights Reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace WpfGtfsRealTime.ViewModels
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Interop;

    using Luminator.Gtfs.Contract;
    using Luminator.Gtfs.IO;
    using Luminator.Metro.Common;
    using Luminator.Metro.Types;
    using Luminator.TransitProvider.GtfsRealTime;
    using Luminator.TransitProvider.Models;
    using Luminator.TransitProvider.Settings;
    using Luminator.Utility;

    using NLog;

    using OxyPlot;

    using Prism.Commands;
    using Prism.Mvvm;

    using transit_realtime;

    using WpfGtfsRealTime.Interfaces;
    using WpfGtfsRealTime.Models;
    using WpfGtfsRealTime.Views;

    using Timer = System.Timers.Timer;

    #endregion

    /// <summary>The prediction window view model.</summary>
    public class PredictionWindowViewModel : BindableBase
    {
        #region Static Fields

        /// <summary>The n logger.</summary>
        private static readonly Logger NLogger = LogManager.GetCurrentClassLogger();

        #endregion

        #region Fields

        /// <summary>The timer.</summary>
        public Timer timer;

        /// <summary>The ui context.</summary>
        private readonly SynchronizationContext uiContext;

        /// <summary>The address.</summary>
        private string address;

        /// <summary>The prediction item collection.</summary>
        private ObservableCollection<GtfsPredictionItem> predictionItemCollection;

        /// <summary>The predictions button text.</summary>
        private string predictionsButtonText;

        /// <summary>The predictions enabled.</summary>
        private bool predictionsEnabled;

        /// <summary>The refresh time.</summary>
        private TimeSpan refreshTime;

        /// <summary>The schedule prediction item collection.</summary>
        private ObservableCollection<GtfsPredictionItem> schedulePredictionItemCollection;

        /// <summary>The selected item schedule.</summary>
        private GtfsPredictionItem selectedItemSchedule;

        /// <summary>The selected stop id.</summary>
        private string selectedStopId;

        /// <summary>The time zone time.</summary>
        private DateTime timeZoneTime;

        /// <summary>The update interval.</summary>
        private int updateInterval;

        /// <summary>The wait for predictions.</summary>
        readonly object waitForPredictions = new object();

        /// <summary>The x axis point.</summary>
        private int xAxisPoint;

     //   private WebPredictionWindow webPredictionWindow;

        #endregion

        #region Constructors and Destructors

        /// <summary>Initializes a new instance of the <see cref="PredictionWindowViewModel" /> class.</summary>
        /// <exception cref="Exception"></exception>
        public PredictionWindowViewModel()
        {
            this.PredictionsCommand = new DelegateCommand(this.GetPredictions, this.CanGetPredictions).ObservesProperty(() => this.Address);
            this.PredictionItemCollection = new ObservableCollection<GtfsPredictionItem>();
            this.SchedulePredictionItemCollection = new ObservableCollection<GtfsPredictionItem>();

            this.SingleClickCommand = new DelegateCommand<object>(this.OnSelectRow, this.CanSelectRow).ObservesProperty(() => this.SchedulePredictionItemCollection);
            this.CloseWindowCommand = new DelegateCommand<object>(this.Close, this.CanClose).ObservesProperty(() => this.Address);
            this.TripSelectedCommand = new DelegateCommand<object>(this.OnSelectTrip, this.CanSelectTrip).ObservesProperty(() => this.SchedulePredictionItemCollection);
            this.StopTimeUpdateSelectedCommand = new DelegateCommand<object>(this.OnSelectStopUpdate, this.CanSelectStopUpdate).ObservesProperty(() => this.StopTimeUpdateCollection);

            this.uiContext = SynchronizationContext.Current;
            var gtfsFeedUrl = UrlLocationsUtil.Instance.Url(TransitProviderType.GtfsRealTime, Constants.CalgaryJurisdiction, UrlType.GtfsRealTime);
            if (string.IsNullOrEmpty(gtfsFeedUrl))
            {
                throw new Exception("Unable to find GtfsRealTime feed URL for jurisdiction " + Constants.CalgaryJurisdiction + "in XML file");
            }

            this.Address = gtfsFeedUrl;

            this.UpdateInterval = 15000;
            this.PredictionsButtonText = "Get Predictions";
            this.timer = new Timer(this.UpdateInterval);

            this.MemoryUsagePlotViewModel = new MemoryUsagePlotViewModel { Points = new ObservableCollection<DataPoint>() };
            this.ThreadUsagePlotViewModel = new ThreadUsagePlotViewModel { Points = new ObservableCollection<DataPoint>() };
            this.RefreshPlotViewModel = new RefreshPlotViewModel { Points = new ObservableCollection<DataPoint>() };
            this.StopTimeUpdateCollection = new ObservableCollection<GtfsPredictionItem>();
            this.CalgaryPilotStops = new ObservableCollection<string>
                                         {
                                             "3857", "9876", "9880", "9877", "9875", "9879", "9878", "3858", "9874", "3816", "3817", "3496", "4738", "3497", "3498", "9767", "4101", "4593", "8463", "4100", "4456", "8468"
                                         };
        }


        #endregion

        #region Public Properties

        /// <summary>Gets or sets the address.</summary>
        public string Address
        {
            get
            {
                return this.address;
            }

            set
            {
                this.SetProperty(ref this.address, value);
            }
        }

        /// <summary>Gets or sets the calgary pilot stops.</summary>
        public ObservableCollection<string> CalgaryPilotStops { get; set; }

        /// <summary>Gets or sets the memory usage plot view model.</summary>
        public MemoryUsagePlotViewModel MemoryUsagePlotViewModel { get; set; }
        public ThreadUsagePlotViewModel ThreadUsagePlotViewModel { get; set; }
        public RefreshPlotViewModel RefreshPlotViewModel { get; set; }

        /// <summary>Gets or sets the prediction item collection.</summary>
        public ObservableCollection<GtfsPredictionItem> PredictionItemCollection
        {
            get
            {
                return this.predictionItemCollection;
            }

            set
            {
                this.SetProperty(ref this.predictionItemCollection, value);
            }
        }

        private ObservableCollection<StopTime> stopTimesCollection;
        public ObservableCollection<StopTime> StopTimesCollection
        {
            get
            {
                return this.stopTimesCollection;
            }

            set
            {
                this.SetProperty(ref this.stopTimesCollection, value);
            }
        }

        private ObservableCollection<Trip> todaysTripsCollection;
        public ObservableCollection<Trip> TodaysTripsCollection
        {
            get
            {
                return this.todaysTripsCollection;
            }

            set
            {
                this.SetProperty(ref this.todaysTripsCollection, value);
            }
        }

        private ObservableCollection<GtfsPredictionItem> stopTimeUpdateCollection;
        public ObservableCollection<GtfsPredictionItem> StopTimeUpdateCollection
        {
            get
            {
                return this.stopTimeUpdateCollection;
            }

            set
            {
                this.SetProperty(ref this.stopTimeUpdateCollection, value);
            }
        }

        /// <summary>Gets or sets the predictions button text.</summary>
        public string PredictionsButtonText
        {
            get
            {
                return this.predictionsButtonText;
            }

            set
            {
                this.SetProperty(ref this.predictionsButtonText, value);
            }
        }

        private string webPredictionUrlForStop;
        public string WebPredictionUrlForStop
        {
            get
            {
                return this.webPredictionUrlForStop;
            }

            set
            {
                this.SetProperty(ref this.webPredictionUrlForStop, value);
            }
        }

        /// <summary>Gets or sets the predictions command.</summary>
        public ICommand PredictionsCommand { get; set; }

        /// <summary>Gets or sets a value indicating whether predictions enabled.</summary>
        public bool PredictionsEnabled
        {
            get
            {
                return this.predictionsEnabled;
            }

            set
            {
                this.SetProperty(ref this.predictionsEnabled, value);
            }
        }

        private bool useAbsDelay;
        public bool UseAbsDelay
        {
            get
            {
                return this.useAbsDelay;
            }

            set
            {
                this.SetProperty(ref this.useAbsDelay, value);
            }
        }

        private bool useTripDirection;
        public bool UseTripDirection
        {
            get
            {
                return this.useTripDirection;
            }

            set
            {
                this.SetProperty(ref this.useTripDirection, value);
            }
        }



        /// <summary>Gets or sets the refresh time.</summary>
        public TimeSpan RefreshTime
        {
            get
            {
                return this.refreshTime;
            }

            set
            {
                this.SetProperty(ref this.refreshTime, value);
            }
        }

        /// <summary>Gets or sets the schedule prediction item collection.</summary>
        public ObservableCollection<GtfsPredictionItem> SchedulePredictionItemCollection
        {
            get
            {
                return this.schedulePredictionItemCollection;
            }

            set
            {
                this.SetProperty(ref this.schedulePredictionItemCollection, value);
            }
        }

        /// <summary>Gets or sets the selected item schedule.</summary>
        public GtfsPredictionItem SelectedItemSchedule
        {
            get
            {
                return this.selectedItemSchedule;
            }

            set
            {
                this.SetProperty(ref this.selectedItemSchedule, value);
            }
        }

        private Trip selectedItemTrip;
        public Trip SelectedItemTrip
        {
            get
            {
                return this.selectedItemTrip;
            }

            set
            {
                this.SetProperty(ref this.selectedItemTrip, value);
            }
        }

        private GtfsPredictionItem selectedItemStopUpdate;
        public GtfsPredictionItem SelectedItemStopUpdate
        {
            get
            {
                return this.selectedItemStopUpdate;
            }

            set
            {
                this.SetProperty(ref this.selectedItemStopUpdate, value);
            }
        }

        /// <summary>Gets or sets the selected stop id.</summary>
        public string SelectedStopId
        {
            get
            {
                return this.selectedStopId;
            }

            set
            {
                this.SetProperty(ref this.selectedStopId, value);
            }
        }

        /// <summary>Gets or sets the single click command.</summary>
        public ICommand SingleClickCommand { get; set; }

        public ICommand TripSelectedCommand { get; set; }
        public ICommand StopTimeUpdateSelectedCommand { get; set; }
        public ICommand CloseWindowCommand { get; set; }

        /// <summary>Gets or sets the time zone time.</summary>
        public DateTime TimeZoneTime
        {
            get
            {
                return this.timeZoneTime;
            }

            set
            {
                this.SetProperty(ref this.timeZoneTime, value);
            }
        }

        /// <summary>Gets or sets the update interval.</summary>
        public int UpdateInterval
        {
            get
            {
                return this.updateInterval;
            }

            set
            {
                this.SetProperty(ref this.updateInterval, value);
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>The gtfs prediction with real time.</summary>
        /// <param name="url">The url.</param>
        /// <param name="jurisdiction">The jurisdiction.</param>
        /// <param name="scheduleOnly">The schedule only.</param>
        /// <param name="reloadSchedule"></param>
        /// <returns>The <see cref="List"/>.</returns>
        public List<TransitProviderPrediction> GtfsPredictionWithRealTime(string url, string jurisdiction, bool scheduleOnly = false, bool reloadSchedule = false)
        {
            var timeZone = UrlLocationsUtil.Instance.GetTimeZone(TransitProviderType.GtfsRealTime, Constants.CalgaryJurisdiction);
            var stops = string.IsNullOrEmpty(this.SelectedStopId) ? this.CalgaryPilotStops.ToList() : new List<string> { this.selectedStopId };
            var predictions = GtfsRealTimePredictions.GtfsPredictionWithRealTime(url, jurisdiction, timeZone, stops, scheduleOnly, false, reloadSchedule, this.UseAbsDelay, this.UseTripDirection);
            var predictionUniqueRoutes = (from p in predictions select p.RouteTag).ToList();
            var realTimeTripsForStop = (from stop in this.StopTimeUpdateCollection select stop.TripId).ToList();
            this.TodaysTripsCollection = new ObservableCollection<Trip>(GtfsRealTimePredictions.FilteredTrips.Where(x => predictionUniqueRoutes.Any(y => y == x.route_id)).Where(z => realTimeTripsForStop.Any( s => s == z.trip_id)).ToList());
            var stopTimes = GtfsRealTimePredictions.AllStopTimes.GetStopTimesInTheNextNHours(1, timeZone).Where(x => x.stop_id == stops.FirstOrDefault()).DistinctBy(m => new { m.arrival_time, m.stop_sequence }).ToList();
            var stopTimesUpdatedByRealTimeTrips = stopTimes.Where(x => realTimeTripsForStop.Any(y => y == x.trip_id)).ToList();
            this.StopTimesCollection = new ObservableCollection<StopTime>(stopTimesUpdatedByRealTimeTrips);
            this.UpdateRealTimeFeedData(stops.FirstOrDefault(), timeZone);
            return predictions;
        }

        public void UpdateRealTimeFeedData(string stopid, string timezone)
        {
            this.uiContext.Send(update => this.StopTimeUpdateCollection.Clear(), null);
            var tempUpdates = new ObservableCollection<GtfsPredictionItem>();
            foreach (var entity in GtfsRealTimePredictions.FilteredFeedEntities)
            {
                var stopTimeUpdates = entity.trip_update.stop_time_update.Where(x => x.stop_id == stopid);
                foreach (var stopTimeUpdate in stopTimeUpdates)
                {
                    var item = new GtfsPredictionItem
                    {
                        TripId = entity.trip_update.trip.trip_id,
                        Direction = (int)entity.trip_update.trip.direction_id,
                        RouteId = entity.trip_update.trip.route_id,
                        RouteName = "NA In RT",
                        StopId = stopTimeUpdate.stop_id,
                        StopName = stopTimeUpdate.stop_sequence.ToString(),//"NA in RT",
                        RealTimeEtaTime = stopTimeUpdate.arrival?.time.ConvertFromEpochTime(timezone) ?? new DateTime()
                    };
                    tempUpdates.Add(item);
                }
            }
            this.uiContext.Send(update => { this.StopTimeUpdateCollection = tempUpdates; }, null);

        }



        #endregion

        #region Methods

        /// <summary>The can get predictions.</summary>
        /// <returns>The <see cref="bool" />.</returns>
        private bool CanGetPredictions()
        {
            var result = !string.IsNullOrEmpty(this.address);
            return result;
        }

        /// <summary>The can select row.</summary>
        /// <param name="arg">The arg.</param>
        /// <returns>The <see cref="bool"/>.</returns>
        private bool CanSelectRow(object arg)
        {
            return true; // this.schedulePredictionItemCollection.Count > 0;
        }

        private bool CanSelectTrip(object arg)
        {
            return true; // this.schedulePredictionItemCollection.Count > 0;
        }

        private bool CanSelectStopUpdate(object arg)
        {
            return true; // this.schedulePredictionItemCollection.Count > 0;
        }

        /// <summary>The get predictions.</summary>
        private void GetPredictions()
        {
            if (this.PredictionsButtonText.Equals("Stop"))
            {
                this.PredictionsButtonText = "Get Predictions";
                this.timer.Stop();
                this.SelectedStopId = null;
                return;
            }

            this.GetPredictionsAndUpdateUI(true);
            if (this.timer != null)
            {
                this.timer.Stop();
                this.timer.Elapsed -= this.TimerElapsed;
                this.timer.Elapsed += this.TimerElapsed;
                this.timer.Interval = this.UpdateInterval;
                this.timer.Start();
                this.PredictionsButtonText = "Stop";
            }
        }

        private object runningTask;
        /// <summary>The get predictions and update ui.</summary>
        private void GetPredictionsAndUpdateUI(bool reloadSchedule = false)
        {
            try
            {
                var timeZone = UrlLocationsUtil.Instance.GetTimeZone(TransitProviderType.GtfsRealTime, Constants.CalgaryJurisdiction);
                this.TimeZoneTime = DateTime.Now.ConvertToTimeZone(timeZone);
                Debug.WriteLine(this.TimeZoneTime);
                //  List<TransitProviderPrediction> schedulePredictions = null;
                List<TransitProviderPrediction> gtfsPredictionWithRealTime = null;
                var sw = new Stopwatch();
                sw.Start();
                runningTask = Task.Run(
                    () =>
                        {
                            gtfsPredictionWithRealTime = this.GtfsPredictionWithRealTime(this.Address, Constants.CalgaryJurisdiction, false, reloadSchedule);
                        }).ContinueWith(
                            previousTask =>
                                {
                                    if (gtfsPredictionWithRealTime != null && (this.uiContext != null ))
                                    {
                                        this.uiContext.Send(update => this.schedulePredictionItemCollection.Clear(), null);
                                        if (gtfsPredictionWithRealTime != null)
                                        {
                                            gtfsPredictionWithRealTime.ForEach(
                                                x =>
                                                    {
                                                        var convertToTimeZone = DateTime.Now.AddMinutes(x.Minutes).ConvertToTimeZone(timeZone);
                                                        var item = new GtfsPredictionItem
                                                        {
                                                            StopId = x.StopId,
                                                            StopName = x.StopName,
                                                            RouteName = x.RouteName,
                                                            RouteId = x.RouteTag,
                                                            TimeUpdated = x.Seconds != 0,
                                                            TripId = x.TripId.ToString(),
                                                            EtaInMin = x.Minutes,
                                                            EtaInMinNonAbs = x.Seconds,
                                                            ScheduledEtaTime = (x.Seconds == 0) ? convertToTimeZone : convertToTimeZone.AddSeconds(-x.Seconds),
                                                            RealTimeEtaTime = convertToTimeZone,
                                                            Direction = x.DirectionNum,
                                                            ArrivalRawText = x.Tag
                                                        };

                                                        this.uiContext.Send(update => this.schedulePredictionItemCollection.Add(item), null);
                                                    });
                                        }


                                        this.uiContext.Send(
                                            update =>
                                                {
                                                    this.RefreshTime = sw.Elapsed;
                                                    sw.Stop();
                                                    NLogger.Info($"Getting data took - {sw.Elapsed.Minutes}:{sw.Elapsed.Seconds}");
                                                    this.xAxisPoint += 10;
                                                    this.MemoryUsagePlotViewModel.Points.Add(new DataPoint(this.xAxisPoint, Math.Log10(Process.GetCurrentProcess().WorkingSet64)));
                                                    if (this.MemoryUsagePlotViewModel.Points.Count > 1000)
                                                    {
                                                        this.MemoryUsagePlotViewModel.Points.RemoveAt(0);
                                                    }
                                                    this.ThreadUsagePlotViewModel.Points.Add(new DataPoint(this.xAxisPoint, Process.GetCurrentProcess().Threads.Count));
                                                    if (this.ThreadUsagePlotViewModel.Points.Count > 1000)
                                                    {
                                                        this.ThreadUsagePlotViewModel.Points.RemoveAt(0);
                                                    }
                                                    this.RefreshPlotViewModel.Points.Add(new DataPoint(this.xAxisPoint, sw.Elapsed.Milliseconds));
                                                    if (this.RefreshPlotViewModel.Points.Count > 1000)
                                                    {
                                                        this.RefreshPlotViewModel.Points.RemoveAt(0);
                                                    }
                                                },
                                            null);
                                    }
                                });// 2 min timeout
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            finally
            {
                Console.WriteLine(" ------------ Done -------------------");
            }
        }

        private void Close(object obj)
        {
            //var predictionWindow = this.webPredictionWindow;
            //if (predictionWindow != null)
            //{
            //    predictionWindow.Close();
            //}
        }

        private bool CanClose(object arg)
        {
            return true;
        }

        private void OnSelectTrip(object arg)
        {
            if (arg != null)
            {
                if (arg.GetType() == typeof(Trip))
                {
                    Trip trip = (Trip)arg;
                    this.uiContext.Send(
                        update =>
                            {
                                this.StopTimeUpdateCollection = new ObservableCollection<GtfsPredictionItem>(this.StopTimeUpdateCollection.Where(x => x.TripId == trip.trip_id && x.RouteId == trip.route_id));
                                this.StopTimesCollection = new ObservableCollection<StopTime>(this.StopTimesCollection.Where(x => x.trip_id == trip.trip_id));
                            }, null);
                   
                }
            }
        }

        private void OnSelectStopUpdate(object arg)
        {
            if (arg != null)
            {
                if (arg.GetType() == typeof(GtfsPredictionItem))
                {
                    GtfsPredictionItem gtfsPredictionItem = (GtfsPredictionItem)arg;
                    this.uiContext.Send(
                        update =>
                        {
                            this.TodaysTripsCollection = new ObservableCollection<Trip>(this.TodaysTripsCollection.Where(x => x.trip_id == gtfsPredictionItem.TripId  && x.route_id == gtfsPredictionItem.RouteId));
                            this.StopTimesCollection = new ObservableCollection<StopTime>(this.StopTimesCollection.Where(x => x.trip_id == gtfsPredictionItem.TripId));
                        }, null);

                }
            }
        }

        /// <summary>The on select row.</summary>
        /// <param name="arg">The arg.</param>
        private
        void OnSelectRow(object arg)
        {
            if (arg != null)
            {
                if (arg.GetType() == typeof(GtfsPredictionItem))
                {
                    var tripid = ((GtfsPredictionItem)arg).TripId;
                    var item = this.SchedulePredictionItemCollection.FirstOrDefault(x => x.TripId == tripid);
                    // this.SelectedItemSchedule = item;
                    if (item != null)
                    {
                        this.WebPredictionUrlForStop = @"http://hastinfoweb.calgarytransit.com/hastinfoweb2//NextDepartures?StopIdentifier=" + item.StopId;
                    }
                    //if (this.webPredictionWindow == null)
                    //{
                    //    this.webPredictionWindow = new WebPredictionWindow { DataContext = this };
                    //    this.webPredictionWindow.Closed += this.WebPredictionWindowClosed;
                    //}
                    //else if (item != null && this.webPredictionWindow != null )
                    //{
                    //    var handle = new WindowInteropHelper(this.webPredictionWindow).EnsureHandle();
                    //    this.WebPredictionUrlForStop = @"http://hastinfoweb.calgarytransit.com/hastinfoweb2//NextDepartures?StopIdentifier=" + item.StopId;
                    //    this.webPredictionWindow.Show();

                    //}
                }
            }
        }

        private void WebPredictionWindowClosed(object sender, EventArgs e)
        {
            //if (this.webPredictionWindow != null)
            //{
            //    this.webPredictionWindow.Closed -= this.WebPredictionWindowClosed;
            //    this.webPredictionWindow = null;
            //}
        }

        /// <summary>The timer elapsed.</summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The e.</param>
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            lock (this.waitForPredictions)
            {
                this.GetPredictionsAndUpdateUI();
            }
        }

        #endregion
    }
}