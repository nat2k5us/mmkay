using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimedSplash
{
    using System.Timers;
    using System.Windows.Threading;

    public partial class SplashScreen : Form
    {
        public Dispatcher CurrentDispatcher { get; }

        public SplashScreen()
        {
            this.InitializeComponent();

            this.CurrentDispatcher = Dispatcher.CurrentDispatcher;

            Timer closeTimer = new Timer(20000);
            closeTimer.Elapsed += CloseTimerElapsed;

            closeTimer.Start();
        }

        private void CloseTimerElapsed(object sender, ElapsedEventArgs e)
        {
            var timer = sender as Timer;
            if (timer != null)
            {
                timer.Stop();
                timer.Dispose();
            }
            this.CurrentDispatcher.BeginInvoke(new Action(this.Close));
        }

    }
}
