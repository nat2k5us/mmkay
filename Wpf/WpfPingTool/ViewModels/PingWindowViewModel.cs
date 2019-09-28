namespace WpfPingTool.ViewModels
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Net;
    using System.Net.NetworkInformation;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;

    using NetoworkLib;

    using NLog;

    using Prism.Commands;
    using Prism.Mvvm;

    using WpfPingTool.Models;

    #endregion

    
    public class PingWindowViewModel : BindableBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string address;

        private bool pingEnabled;

        private ObservableCollection<PingItem> pingItemCollection;

        public ObservableCollection<PingItem> PingItemCollection
        {
            get { return this.pingItemCollection; }
            set { this.SetProperty(ref this.pingItemCollection, value); }
        }
        public PingWindowViewModel()
        {
            this.PingCommand = new DelegateCommand(this.PingHost, this.CanPingHost).ObservesProperty(() => this.Address);
            this.PingItemCollection = new ObservableCollection<PingItem>();
            var host = NetworkUtils.GetLocalIpAddress().ToString();
            long time = 0;
            var result = NetworkUtils.PingHost(NetworkUtils.GetLocalIpAddress().ToString(), out time);
           
            Logger.Info("Testing");
            this.address = host;
        }

        private bool CanPingHost()
        {
            bool result = false;
            if (!string.IsNullOrEmpty(this.address))
            {
                IPAddress ipAfterParse;
                result = IPAddress.TryParse(this.address, out ipAfterParse);
            }
            return result;
        }

        private void PingHost()
        {
            IPAddress ipAddress;
            var hostsList = new List<string>();
            if (IPAddress.TryParse(this.address, out ipAddress))
            {
                ipAddress = ipAddress.SetLastOctetTo(0);
                for (int i = 0; i < 255; i++)
                {
                    ipAddress = NetworkUtils.IncrementIpAddress(ipAddress);
                    hostsList.Add(ipAddress.ToString());
                }
                var uiContext = SynchronizationContext.Current;
                try
                {
                   Task.Run(() => {
                            foreach (var result in hostsList
                             .AsParallel().WithDegreeOfParallelism(64).Select(site => new { site, p = new Ping().Send(site) }).Select(
                                        @t => new
                                            {
                                                @t.site,
                                                Result = @t.p.Status,
                                                Time = @t.p.RoundtripTime
                                            }))
                                {

                                  var item = new PingItem { Host = result.site, Result = result.Result, Time = result.Time };
                                  Logger.Info($" Ping Item {result}");
                                  uiContext.Send(x => this.PingItemCollection.Add(item), null);
                                }
                            });

                }
                catch (OperationCanceledException exception)
                {
                    Logger.Info($" Operation was cancelled. {exception.Message}");
                }
                catch (Exception exception)
                {
                    Logger.Info(exception);
                }
               
            }

           // NetworkUtils.PingHost(this.address);
        }

        public string Address
        {
            get { return this.address; }
            set { this.SetProperty(ref this.address, value); }
        }

        public bool PingEnabled
        {
            get { return this.pingEnabled; }
            set { this.SetProperty(ref this.pingEnabled, value); }
        }

        public ICommand PingCommand { get; set; }
    }

   
}