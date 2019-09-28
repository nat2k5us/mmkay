namespace WpfPlot
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Timers;

    using OxyPlot;
    using OxyPlot.Series;

    using Timer = System.Timers.Timer;

    public class MainViewModel
    {

        /// <summary>The ui context.</summary>
        private readonly SynchronizationContext uiContext;
        public System.Timers.Timer UpdateTimer;

        private static Random random = new Random();

        public string Title { get; private set; }

        public ObservableCollection<DataPoint> Points { get; private set; }

        private int counterX = 0;
        public MainViewModel()
        {
            //this.Title = "Example 2";
          

            this.uiContext = SynchronizationContext.Current;
            this.Points = new ObservableCollection<DataPoint>();
            this.UpdateTimer = new Timer(1000);
            this.UpdateTimer.Elapsed += UpdateTimerElapsed;
            this.UpdateTimer.Start();
        }

        private void UpdateTimerElapsed(object sender, ElapsedEventArgs e)
        {
           this.uiContext.Send(
                x =>
                    {
                        this.Points.Add(new DataPoint(this.counterX++, this.MemUsage(this.counterX)));
                        if(this.Points.Count > 40) this.Points.RemoveAt(0);
                    }, null);
        }

        public double MemUsage(double param)
        {
            return Math.Log10(Process.GetCurrentProcess().WorkingSet64); //random.Next(15); //Process.GetCurrentProcess().WorkingSet64;
        }

        public PlotModel MyModel { get; private set; }
    }
}
