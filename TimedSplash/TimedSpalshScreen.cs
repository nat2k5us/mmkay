namespace TimedSplash
{
    #region

    using System;
    using System.Drawing;
    using System.Timers;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Linq;

    using ExtendedXmlSerialization;

    using KeyboardMouse;

    using NLog;

    using Timer = System.Timers.Timer;

    #endregion

    //public class TimedSpalshScreenConfig : ExtendedXmlSerializerConfig<TimedSpalshScreen>
    //{
    //    public TimedSpalshScreenConfig()
    //    {
    //        this.CustomSerializer(this.Serializer, this.Deserialize);
    //    }

    //    public TimedSpalshScreen  Deserialize(XElement element)
    //    {
    //        return new TimedSpalshScreen(element.Element("String").Value);
    //    }

    //    public void Serializer(XmlWriter writer, TimedSpalshScreen obj)
    //    {
    //        writer.WriteElementString("String", obj.FontName);
    //    }
    //}
    public class TimedSpalshScreen
    {
        private Timer closeCountDownTimer;
        private Timer closeTimer;
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public string FontName { get; set; }
        public TimedSpalshScreen()
        {
        }

        public TimedSpalshScreen(Rectangle dimensions, double opacity, Dispatcher currentDispatcher)
        {
            try
            {
                this.TimedSplashScreenForm = new Form
                                                 {
                                                     Opacity = opacity,
                                                     ControlBox = false,
                                                     Text = string.Empty,
                                                     FormBorderStyle = FormBorderStyle.None,
                                                     TopMost = true
                                                 };
                this.Messagelabel = new Label
                                        {
                                            AutoSize = true,
                                            Visible = true,
                                            Font = new Font("Arial", 14, FontStyle.Bold),
                                            MaximumSize = new Size(0,100)
                                        };
                this.ProgressBar = new ProgressBar
                                       {
                                           Maximum = 10,
                                           Step = 1,
                                           Visible = true
                                       };
                this.TimedSplashScreenForm.Controls.Add(this.Messagelabel);
                this.TimedSplashScreenForm.Controls.Add(this.ProgressBar);
                this.ProgressBar.Location = new Point((dimensions.Width / 2) - (this.ProgressBar.Width / 2), dimensions.Height / 2);
                this.CurrentDispatcher = currentDispatcher;
                WindowList.MoveWindow(this.TimedSplashScreenForm.Handle, dimensions.X + dimensions.Width - dimensions.Width/3, dimensions.Y, dimensions.Width/3, dimensions.Height/6, true);
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + e.InnerException?.Message);
            }
           
        }

        public Form TimedSplashScreenForm { get; set; }

        public Dispatcher CurrentDispatcher { get; }

        private Label Messagelabel { get; set; }

        private ProgressBar ProgressBar { set; get; }

        private string DisplayedMessage { get; set; }

        private int Countdown { get; set; }

        public void ShowSplashScreen(string message = "", int showForMilliseconds = 1000)
        {
            try
            {
                this.Messagelabel.Text = this.DisplayedMessage = message;
                this.Messagelabel.Location = new Point(10 , this.TimedSplashScreenForm.Height / 8);
                this.ProgressBar.Location = new Point(20 , this.TimedSplashScreenForm.Height / 2);
                this.ProgressBar.Maximum = showForMilliseconds / 1000;
                this.closeTimer = new Timer(showForMilliseconds);
                this.Countdown = showForMilliseconds / 1000;
                this.closeCountDownTimer = new Timer(1000);
                this.closeCountDownTimer.Elapsed += this.CloseCountDownTimer_Elapsed;
                this.closeCountDownTimer.Start();
                this.closeTimer.Elapsed += this.CloseTimerElapsed;
                this.closeTimer.Start();
                var currentDispatcher = this.CurrentDispatcher;
                this.TimedSplashScreenForm.Visible = true;
                currentDispatcher?.BeginInvoke(new Action(() => this.TimedSplashScreenForm.Show()));
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + e.InnerException?.Message);
            }
        }

        public void UpdateMessage(string message)
        {
            this.Messagelabel.Text = this.DisplayedMessage = message;
            Logger.Info(message);
        }

        private void CloseCountDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var currentDispatcher = this.CurrentDispatcher;
            currentDispatcher?.BeginInvoke(
                new Action(
                    () =>
                        {
                            var updatedMessage = $"{this.Countdown--}:{this.DisplayedMessage}";
                            this.Messagelabel.Text = updatedMessage;
                            this.ProgressBar.PerformStep();
                        }));
        }

        public void Close()
        {
            try
            {
                var countDownTimer = this.closeCountDownTimer;
                if (countDownTimer != null)
                {
                    countDownTimer.Elapsed -= this.CloseCountDownTimer_Elapsed;
              
                    countDownTimer?.Stop();
                    countDownTimer?.Dispose();
                    this.closeCountDownTimer = null;
                }
                var timer = this.closeTimer;
                if (timer != null)
                {
                    timer.Elapsed -= this.CloseTimerElapsed;
                    timer?.Stop();
                    timer?.Dispose();
                    this.closeTimer = null;
                }
          
                var currentDispatcher = this.CurrentDispatcher;
                currentDispatcher?.BeginInvoke(new Action(() => this.TimedSplashScreenForm.Close()));
            }
            catch (Exception e)
            {
                Logger.Info(e.Message + e.InnerException?.Message);
            }
        }

        private void CloseTimerElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                var timer = sender as Timer;
                if (timer != null)
                {
                    this.closeCountDownTimer.Elapsed -= this.CloseCountDownTimer_Elapsed;
                    this.closeTimer.Elapsed -= this.CloseTimerElapsed;
                    this.closeCountDownTimer?.Stop();
                    this.closeCountDownTimer?.Dispose();
                    this.closeCountDownTimer = null;
                    timer.Stop();
                    timer.Dispose();
                }
                var currentDispatcher = this.CurrentDispatcher;
                currentDispatcher?.BeginInvoke(new Action(() => this.TimedSplashScreenForm.Close()));
            }
            catch (Exception exception)
            {
                Logger.Info(exception.Message + exception.InnerException?.Message);
            }
        }
    }
}