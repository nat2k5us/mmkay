namespace FindImageInImage
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using System.Windows.Threading;

    using AudioSamples;

    using FindImageInImage.Properties;

    using KeyboardMouse;

    using NLog;

    using PtcAdProcessor;

    using ScreenShots;

    using TimedSplash;

    using Utilities;

    public partial class FindImageInScreenShot : Form, ISplashMessage
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly List<Rectangle> alreadyProcessedMegaRects = new List<Rectangle>();

        private readonly Random rand = new Random(Guid.NewGuid().GetHashCode());

        private readonly ScreenShotsManager screenShotsManager;

        private bool addingImageClixSenseClose;

        private bool addingImageClixSenseFind;

        private bool addingImageNemoClose;

        private bool addingImageNemoDot;

        private bool addingImageNemoFind;

        private List<Rectangle> alreadyProcessedNemoRects = new List<Rectangle>();

        private Dictionary<string, Bitmap> catBitmaps;

        private ClixSenseAdProcessor clixSenseAdProcessor;

        private CustomWindow customWindow = new CustomWindow("Test Class");

        private MegaAdProcessor megaAdProcessor;

        private PtcAdProcessor nemoAdProcessor;

        // Returns a WindowInformation objects representing the Desktop window with all
        // of the information returned above. The objects also contains all of the other 
        // windows in the ChildWindows property. This can be useful for building a tree
        // view representation of the windows.
        private WindowInformation windowInformationTree = WindowList.GetAllWindowsTree();

        // Returns a list of WindowInformation objects with Handle, Caption, Class
        private readonly List<WindowInformation> windowListBasic = WindowList.GetAllWindows();

        // Returns a list of WindowInformation objects with Handle, Caption, Class,
        // Parent, Children, Siblings and process information
        private List<WindowInformation> windowListExtended = WindowList.GetAllWindowsExtendedInfo();

        // private List<string> words = commaWords.Split(',');
        public FindImageInScreenShot()
        {
            this.InitializeComponent();
            this.screenShotsManager = new ScreenShotsManager();
            InterceptKeys.OnKeyDown += this.InterceptKeys_OnKeyDown;
            InterceptKeys.Start();
            InterceptMouse.OnMouseLeftButtonDown += this.InterceptMouse_OnMouseLeftButtonDown;

            InterceptMouse.Start();
        }

        public bool FollowMouse { get; private set; }

        public TimedSpalshScreen SplashScreen { get; set; }

        private Screen CurrentScreen { get; set; }

        public void CloseSplash()
        {
            Dispatcher.CurrentDispatcher?.BeginInvoke(
                new Action(
                    () =>
                        {
                            var timedSplashScreen = this.SplashScreen;
                            timedSplashScreen?.Close();
                        }));
        }

        public void ShowSplashMessage(Point point, string message, int showForMilliseconds)
        {
            this.SplashScreen?.Close();
            point.Offset(100, 100);
            var screen = Screen.FromPoint(point);
            Dispatcher.CurrentDispatcher?.BeginInvoke(
                new Action(
                    () =>
                        {
                            this.SplashScreen = new TimedSpalshScreen(screen.Bounds, 0.9, Dispatcher.CurrentDispatcher);
                            this.SplashScreen.ShowSplashScreen(message, showForMilliseconds);
                        }));
        }

        public void UpdateSplashMessage(string message)
        {
            Dispatcher.CurrentDispatcher?.BeginInvoke(
                new Action(
                    () =>
                        {
                            var timedSplashScreen = this.SplashScreen;
                            timedSplashScreen?.UpdateMessage(message);
                        }));
        }

        protected override void OnLoad(EventArgs e)
        {
            var screen = Screen.FromPoint(this.Location);
            this.Location = new Point(screen.WorkingArea.Right - this.Width, screen.WorkingArea.Bottom - this.Height);
            base.OnLoad(e);
        }

        private void buttonAddSenseClose_Click(object sender, EventArgs e)
        {
            this.addingImageClixSenseClose = true;
        }

        private void buttonAddSenseFind_Click(object sender, EventArgs e)
        {
            this.addingImageClixSenseFind = true;
        }

        private void buttonAdPrize_Click(object sender, EventArgs e)
        {
            var nemoFindBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoFindDir);
            var nemoCloseBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoCloseDir);
            var nemoDotBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoDotDir);
            this.nemoAdProcessor = new NemoAdProcessor(
                this.screenShotsManager,
                nemoFindBitmaps,
                nemoDotBitmaps,
                nemoCloseBitmaps,
                Settings.Default.NemoScreenIndex,
                this.checkBoxFakeMoves.Checked,
                Dispatcher.CurrentDispatcher);

            var task = Task.Factory.StartNew(() => { this.nemoAdProcessor.GamePrize(); });
            var continueWith = task.ContinueWith(antecedent => { this.nemoAdProcessor.DoAdProcessing(); });
        }

        private void buttonArrangeWindows_Click(object sender, EventArgs e)
        {
            // Using a lambda expression
            // WindowInformation wi = windowListExtended.Find(
            // w => w.Caption.StartsWith("c# - How")
            // );
            var windowsToBeArranged = new List<IntPtr>();
            var windowsListSecondSet = new List<IntPtr>();
            this.windowListBasic.ForEach(x => Logger.Info($" {x.Class} - {x.Caption} "));
            foreach (var windowInformation in this.windowListBasic)
            {
                if (windowInformation.Class.Contains("SunAwtFrame") && windowInformation.Caption.Contains("5 min bars") && windowInformation.Caption.Contains("@"))
                {
                    windowsToBeArranged.Add(windowInformation.Handle);
                }

                // if (windowInformation.Class.Contains("Chrome_WidgetWin_1") && !string.IsNullOrEmpty(windowInformation.Caption))
                // {
                // windowsToBeArranged.Add(windowInformation.Handle);
                // }
            }

            WindowList.TileWindows(windowsToBeArranged, WindowList.WindowTile.Horizontally, 6, 4);

            foreach (var windowInformation in this.windowListBasic)
            {
                if (windowInformation.Class.Contains("SunAwtFrame") && windowInformation.Caption.Contains("5 min bars"))
                {
                    windowsListSecondSet.Add(windowInformation.Handle);
                }

                // if (windowInformation.Class.Contains("Chrome_WidgetWin_1") && !string.IsNullOrEmpty(windowInformation.Caption))
                // {
                // windowsToBeArranged.Add(windowInformation.Handle);
                // }
            }

            windowsListSecondSet = windowsListSecondSet.Except(windowsToBeArranged).ToList();

            WindowList.TileWindows(windowsListSecondSet, WindowList.WindowTile.Vertically, 8, 3, 2);
            // WindowInformation w2 = windowListBasic.Find(
            // w => w.Class.Contains("SunAwtFrame") && !w.Caption.Contains("5 min bars")
            // );

            // Using a query expression
            // WindowInformation wi1 = (from w in windowListExtended.AsEnumerable()
            // where w.Caption.StartsWith("c# - How")
            // select w).First();
        }

        private void buttonClixGridPrize_Click(object sender, EventArgs e)
        {
            var senseFindBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.ClickSenseFindDir);
            var senseCloseBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.ClickSenseCloseDir);
            this.clixSenseAdProcessor = new ClixSenseAdProcessor(
                this.screenShotsManager,
                senseFindBitmaps,
                this.catBitmaps,
                senseCloseBitmaps,
                Settings.Default.ClickSenseScreenIndex,
                Settings.Default.ScreenZoomFactorX,
                Settings.Default.ScreenZoomFactorY,
                this.checkBoxFakeMoves.Checked,
                Dispatcher.CurrentDispatcher);
            var endGame = @".\\ClixGridEnd\\endGame.bmp";
            if (!File.Exists(endGame))
            {
                this.ShowSplashMessage(new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2), $"File did not exist {endGame}", 3000);
                return;
            }

            var bmpEndGame2Find = (Bitmap)Image.FromFile(endGame);
            try
            {
                var senseTask = Task.Run(
                    () =>
                        {
                            this.clixSenseAdProcessor.GamePrize(bmpEndGame2Find);

                            // ThreadHelpers.ShowThreadInformation($"Main Task(Task #{Task.CurrentId})");
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

        private void buttonFindClickingDot_Click(object sender, EventArgs e)
        {
            this.addingImageNemoDot = true;
        }

        private void buttonFindClosingImage_Click(object sender, EventArgs e)
        {
            this.addingImageNemoClose = true;
        }

        private void buttonFindMega_Click(object sender, EventArgs e)
        {
            var megaFindBitmaps = new List<Bitmap> { Resources.megabux };
            var megaCloseBitmaps = new List<Bitmap>
                                       {
                                           Resources.megaclose,
                                           Resources.Invalid,
                                           Resources.loadfialmega,
                                           Resources.busty,
                                           Resources.donkey,
                                           Resources.eyepop,
                                           Resources.ghost,
                                           Resources.kfc,
                                           Resources.Mario,
                                           Resources.penguin,
                                           Resources.robo,
                                           Resources.sheep,
                                           Resources.shrek
                                       };
            this.megaAdProcessor = new MegaAdProcessor(
                this.screenShotsManager,
                megaFindBitmaps,
                megaCloseBitmaps,
                this.checkBoxFakeMoves.Checked);

            try
            {
                Task.Run(() => { this.megaAdProcessor.DoAdProcessing(); });
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

        private void buttonFindNemo_Click(object sender, EventArgs e)
        {
            var nemoFindBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoFindDir);
            var nemoCloseBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoCloseDir);
            var nemoDotBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoDotDir);
            this.nemoAdProcessor = new NemoAdProcessor(
                this.screenShotsManager,
                nemoFindBitmaps,
                nemoDotBitmaps,
                nemoCloseBitmaps,
                Settings.Default.NemoScreenIndex,
                this.checkBoxFakeMoves.Checked,
                Dispatcher.CurrentDispatcher);

            try
            {
                Task.Run(() => { this.nemoAdProcessor.DoAdProcessing(); }).ContinueWith((prev) => { Console.WriteLine("Done"); });
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

        private void buttonFindNemoAd_Click(object sender, EventArgs e)
        {
            this.addingImageNemoFind = true;
        }

        private void buttonFindSense_Click(object sender, EventArgs e)
        {
            var senseFindBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.ClickSenseFindDir);
            var senseCloseBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.ClickSenseCloseDir);
            this.catBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectoryAsDictionary(@".\\Cats\\");
            this.clixSenseAdProcessor = new ClixSenseAdProcessor(
                this.screenShotsManager,
                senseFindBitmaps,
                this.catBitmaps,
                senseCloseBitmaps,
                Settings.Default.ClickSenseScreenIndex,
                Settings.Default.ScreenZoomFactorX,
                Settings.Default.ScreenZoomFactorY,
                this.checkBoxFakeMoves.Checked,
                Dispatcher.CurrentDispatcher);

            try
            {
                var senseTask = Task.Run(
                    () =>
                        {
                            this.clixSenseAdProcessor.DoAdProcessing();
                            ThreadHelpers.ShowThreadInformation($"Main Task(Task #{Task.CurrentId})");
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

        private void buttonFollowMouse_Click(object sender, EventArgs e)
        {
            this.FollowMouse = true;
        }

        private void buttonReIndexFiles_Click(object sender, EventArgs e)
        {
            FolderUtils.ReindexFilesInFolder(this.Handle);
        }

        private void buttonShowSplash_Click(object sender, EventArgs e)
        {
            var nemoFindBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoFindDir);
            var nemoCloseBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoCloseDir);
            var nemoDotBitmaps = this.screenShotsManager.LoadAllBitmapsFromDirectory(Settings.Default.NemoDotDir);
            this.nemoAdProcessor = null;
            this.nemoAdProcessor = new NemoAdProcessor(
                this.screenShotsManager,
                nemoFindBitmaps,
                nemoDotBitmaps,
                nemoCloseBitmaps,
                Settings.Default.NemoScreenIndex,
                this.checkBoxFakeMoves.Checked,
                Dispatcher.CurrentDispatcher);
            try
            {
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

        private void FindImageInScreenShot_FormClosing(object sender, FormClosingEventArgs e)
        {
            InterceptKeys.Stop();
            InterceptMouse.Stop();
            AudioPlaybackEngine.Instance.Dispose();
        }

        private void FindImageInScreenShot_Load(object sender, EventArgs e)
        {
        }

        private void InterceptKeys_OnKeyDown(object sender, KeyEventArgs e)
        {
            Logger.Info($"Key Pressed : {e.KeyCode} key press after {IdleTimeFinder.GetIdleTime()}");
            if (e.KeyCode == Keys.Escape)
            {
                var senseAdProcessor = this.clixSenseAdProcessor;
                if (senseAdProcessor != null)
                {
                    senseAdProcessor.StopAllProcessing = true;
                    senseAdProcessor.CaptureCatImage = false;
                }

                var ptcAdProcessor = this.nemoAdProcessor;
                if (ptcAdProcessor != null) ptcAdProcessor.StopAllProcessing = true;
                Logger.Info($"{Resources.FindImageInScreenShot_InterceptKeys_OnKeyDown_Stopping_all_Processing}");
                this.notifyIcon1.Icon = SystemIcons.Exclamation;
                this.notifyIcon1.BalloonTipTitle = "Stop";
                this.notifyIcon1.BalloonTipText = Resources.FindImageInScreenShot_InterceptKeys_OnKeyDown_Stopping_all_Processing;
                this.notifyIcon1.BalloonTipIcon = ToolTipIcon.Warning;
                this.notifyIcon1.ShowBalloonTip(5000);
            }
        }

        private void InterceptMouse_OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            Logger.Info($"Mouse Button Pressed : {e.Button}");
            if (e.Button == MouseButtons.Left)
            {
                Logger.Info($" Location :  {e.X} ,  {e.Y} clicked after {IdleTimeFinder.GetIdleTime()} minutes");
                if (this.FollowMouse)
                {
                    this.CurrentScreen = Screen.FromPoint(new Point(e.X, e.Y));
                    var splashScreen = new TimedSpalshScreen(this.CurrentScreen.WorkingArea, 0.7, Dispatcher.CurrentDispatcher);
                    splashScreen.ShowSplashScreen("Counting down", 10000);
                    this.FollowMouse = false;
                }

                if (this.addingImageClixSenseFind)
                {
                    this.screenShotsManager.AddToFindImages(e, Settings.Default.ClickSenseFindDir);
                    this.addingImageClixSenseFind = false;
                }

                if (this.addingImageClixSenseClose)
                {
                    this.screenShotsManager.AddToFindImages(e, Settings.Default.ClickSenseCloseDir);
                    this.addingImageClixSenseClose = false;
                }

                if (this.addingImageNemoFind)
                {
                    this.screenShotsManager.AddToFindImages(e, Settings.Default.NemoFindDir);
                    this.addingImageNemoFind = false;
                }

                if (this.addingImageNemoDot)
                {
                    this.screenShotsManager.AddToFindImages(e, Settings.Default.NemoDotDir);
                    this.addingImageNemoDot = false;
                }

                if (this.addingImageNemoClose)
                {
                    this.screenShotsManager.AddToFindImages(e, Settings.Default.NemoCloseDir);
                    this.addingImageNemoClose = false;
                }

                var senseAdProcessor = this.clixSenseAdProcessor;

                if (senseAdProcessor != null)
                {
                    senseAdProcessor?.SaveSenseCatImageToDb(e);
                    senseAdProcessor.CaptureCatImage = false;
                    Logger.Info($" {nameof(senseAdProcessor.CaptureCatImage)} : {senseAdProcessor.CaptureCatImage}");
                }
            }
        }

        private void ButtonCloseTestClick(object sender, EventArgs e)
        {
            var location = new Point(
                (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2);
            var shotcBmp = @"shotc.bmp";
            var nemoCloseBmp = @"1.bmp";
            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo != null) Console.WriteLine(directoryInfo.FullName);
            var hayStackShotPath = Path.Combine(Directory.GetCurrentDirectory(), shotcBmp);
            var needlePath = Path.Combine(Directory.GetCurrentDirectory(), "NemoCloses", nemoCloseBmp);
            this.ShowSplashMessage(location, File.Exists(hayStackShotPath) ? $"  {hayStackShotPath} Exists" : $" {hayStackShotPath} does not Exist", 4000);
            this.UpdateSplashMessage(File.Exists(needlePath) ? $"  {needlePath} Exists" : $" {needlePath} does not Exist");

            var haystackBitmap = (Bitmap)Image.FromFile(hayStackShotPath);
            var needleBitmap = (Bitmap)Image.FromFile(needlePath);
            var shotsManager = this.screenShotsManager;
            if (shotsManager != null)
            {
                var result = shotsManager.Find(haystackBitmap, needleBitmap);
                this.UpdateSplashMessage($"Close found at {result}");
            }
        }
    }
}