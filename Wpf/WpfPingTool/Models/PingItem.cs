namespace WpfPingTool.Models
{
    using System;
    using System.Net.NetworkInformation;

    using Prism.Mvvm;

    public class PingItem : BindableBase
    {
        private string host;

        private IPStatus result;

        private long time;

        public string Host
        {
            get { return this.host; }
            set { this.SetProperty(ref this.host, value); }
        }

        public IPStatus Result
        {
            get { return this.result; }
            set { this.SetProperty(ref this.result, value); }
        }

        public long Time
        {
            get { return this.time; }
            set { this.SetProperty(ref this.time, value); }
        }

        public override string ToString()
        {
            return $"Host : {this.Host}, Result: {this.Result} , Time: {this.Time}";
        }
    }
}
