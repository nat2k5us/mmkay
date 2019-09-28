﻿//      *********    DO NOT MODIFY THIS FILE     *********
//      This file is regenerated by a design tool. Making
//      changes to this file can cause errors.
namespace Expression.Blend.SampleData.SamplePingData
{
    using System; 
    using System.ComponentModel;

// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
    internal class SamplePingData { }
#else

    public class SamplePingData : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public SamplePingData()
        {
            try
            {
                Uri resourceUri = new Uri("/WpfPingTool;component/SampleData/SamplePingData/SamplePingData.xaml", UriKind.RelativeOrAbsolute);
                System.Windows.Application.LoadComponent(this, resourceUri);
            }
            catch
            {
            }
        }

        private PingResultCollection _PingResultCollection = new PingResultCollection();

        public PingResultCollection PingResultCollection
        {
            get
            {
                return this._PingResultCollection;
            }
        }
    }

    public class PingResultCollection : System.Collections.ObjectModel.ObservableCollection<PingResultCollectionItem>
    { 
    }

    public class PingResultCollectionItem : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _Host = string.Empty;

        public string Host
        {
            get
            {
                return this._Host;
            }

            set
            {
                if (this._Host != value)
                {
                    this._Host = value;
                    this.OnPropertyChanged("Host");
                }
            }
        }

        private bool _Result = false;

        public bool Result
        {
            get
            {
                return this._Result;
            }

            set
            {
                if (this._Result != value)
                {
                    this._Result = value;
                    this.OnPropertyChanged("Result");
                }
            }
        }

        private double _Time = 0;

        public double Time
        {
            get
            {
                return this._Time;
            }

            set
            {
                if (this._Time != value)
                {
                    this._Time = value;
                    this.OnPropertyChanged("Time");
                }
            }
        }
    }
#endif
}
