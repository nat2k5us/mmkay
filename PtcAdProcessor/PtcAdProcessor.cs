namespace PtcAdProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Threading;

    using WindowsInput;
    using WindowsInput.Native;

    using KeyboardMouse;

    using NLog;

    using ProcessManagement;

    using ScreenShots;

    using SpeechLibrary;

    using TimedSplash;

    public abstract class PtcAdProcessor : ISplashMessage
    {
        
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly Random rand = new Random(Guid.NewGuid().GetHashCode());

        protected KeyboardSimulator keyboardSimulator = new KeyboardSimulator(new InputSimulator());

        protected MouseSimulator mouseSimulator = new MouseSimulator(new InputSimulator());

        protected  List<Bitmap> closeBitmaps;

        protected  bool fakeMoves;

        public Rectangle DisplayScreen;

        public bool StopAllProcessing;

        protected  ScreenShotsManager screenShotsManager;

        protected  List<Bitmap> toFindBitmaps;

        protected Talking TalkingCool;


        protected Dispatcher currentDispatcher;

        protected PtcAdProcessor(Dispatcher currentDispatcher)
        {
            this.currentDispatcher = Dispatcher.CurrentDispatcher;
        }

        protected PtcAdProcessor()
        {
        }

        public abstract bool FindAd();

        public void Refresh()
        {
            MouseInput.MoveMouse(this.DisplayScreen.X + this.DisplayScreen.Width / 2 + this.rand.Next(-50, 100),
                                           this.DisplayScreen.Y + this.rand.Next(0, 30));
            this.mouseSimulator.LeftButtonClick();
            this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.F5);
            Logger.Info($" Keyboard Refresh ");
            Thread.Sleep(this.rand.Next(3000, 5000));
           

        }

        public void PageDown()
        {
            MouseInput.MoveMouse(this.DisplayScreen.X + this.DisplayScreen.Width / 2 + this.rand.Next(50, 100),
                                           this.DisplayScreen.Y + this.rand.Next(0, 20));
            this.mouseSimulator.LeftButtonClick();
            this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.NEXT);
            Logger.Info($" Keyboard Refresh ");
            Thread.Sleep(this.rand.Next(100, 1000));
        }
        public void PageUp()
        {
            MouseInput.MoveMouse(this.DisplayScreen.X + this.DisplayScreen.Width / 2 + this.rand.Next(50, 100),
                                           this.DisplayScreen.Y + this.rand.Next(0, 20));
            this.mouseSimulator.LeftButtonClick();
            this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.PRIOR);
            Logger.Info($" Keyboard Refresh ");
            Thread.Sleep(this.rand.Next(100, 1000));
        }

        public void Close()
        {
            this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
            Logger.Info($" Keyboard Close");
            
        }

        public TimedSpalshScreen SplashScreen { get; set; }

        public void ShowSplashMessage(Point point, string message, int showForMilliseconds)
        {
            this.SplashScreen?.Close();
            point.Offset(100, 100);
            var screen = Screen.FromPoint(point);
            this.currentDispatcher?.BeginInvoke(
                new Action(
                    () =>
                        {
                            this.SplashScreen = new TimedSpalshScreen(screen.Bounds, 0.9, this.currentDispatcher);
                            this.SplashScreen.ShowSplashScreen(message, showForMilliseconds);
                        }));
        }

        public void UpdateSplashMessage(string message)
        {
            this.currentDispatcher?.BeginInvoke(
             new Action(
                 () =>
                     {
                         var timedSpalshScreen = this.SplashScreen;
                         timedSpalshScreen?.UpdateMessage(message);
                     }));
        }

        public void CloseSplash()
        {
            this.currentDispatcher?.BeginInvoke(
             new Action(
                 () =>
                 {
                     this.SplashScreen.Close();
                 }));
        }


        public abstract bool GamePrize();
        public void DoAdProcessing()
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name}");
            var scrollInc = 10;
            var scrollCurrent = 120;
            var millisecondsIn3Hours = 12 * 60 * 60 * 1000;
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            while (true)
            {
                if (this.StopAllProcessing) return;
                if (stopwatch.ElapsedMilliseconds > millisecondsIn3Hours)
                    break;
                Logger.Info($"Time Elapsed: {stopwatch.ElapsedMilliseconds / (1000 * 60)} minutes.");
                if (!this.FindAd())
                {
                    Logger.Info($" Ad Not Found .......");
                    MouseInput.MoveMouse(this.DisplayScreen.X + this.DisplayScreen.Width  + this.rand.Next(-150, -100),
                                          this.DisplayScreen.Y + this.DisplayScreen.Height/2 + this.rand.Next(0, 30));
                    if (scrollCurrent >= 120) scrollInc = -10;
                    if (scrollCurrent <= 0)
                    {
                       this.Refresh();
                        scrollCurrent = 120;
                        // scrollInc = 10; // always scroll down - refresh scrolls up to top
                    }
                    scrollCurrent += scrollInc;
                    MouseInput.ScrollWheel(scrollInc);
                    Thread.Sleep(3000);
                    Logger.Info(
                        scrollInc < 0
                            ? $"Scrolling down to find an Ad by {scrollInc}...{scrollCurrent}"
                            : $"Scrolling up to find an Ad by {scrollInc}...{scrollCurrent}");
                }
                else
                {
                   Logger.Info($" Found a target .......");
                }
            }
        }
    }
}