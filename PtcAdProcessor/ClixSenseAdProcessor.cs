namespace PtcAdProcessor
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Speech.Recognition;
    using System.Speech.Synthesis;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms;
    using System.Windows.Threading;
    using System.Xml.Serialization;

    using WindowsInput.Native;

    using AudioSamples;

    using KeyboardMouse;

    using NLog;

    using ProcessManagement;

    using ScreenShots;

    using Utilities;

    public class ClixSenseAdProcessorConfig
    {
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        public ClixSenseAdProcessorConfig()
        {
            this.CatCapthaTopLeft = new Point(0, 0);
            this.CatCapthaBottomRight = new Point(1050, 450);
            this.ClixGridTopLeft = new Point(356, 481);
            this.ClixGridBottomRight = new Point(1152, 1000);
            this.AdClickXRange = this.AdClickYRange = 30;
            this.ClixGridXDivisions = 30;
            this.ClixGridYDivisions = 20;
        }

        public Point CatCapthaTopLeft { get; set; }

        public Point CatCapthaBottomRight { get; set; }

        public Point ClixGridTopLeft { get; set; }

        public Point ClixGridBottomRight { get; set; }

        public int ClixGridXDivisions { get; set; }

        public int ClixGridYDivisions { get; set; }

        public int AdClickXRange { get; set; }

        public int AdClickYRange { get; set; }

        public bool Save(string path)
        {
            try
            {
                var xs = new XmlSerializer(typeof(ClixSenseAdProcessorConfig));
                TextWriter tw = new StreamWriter(path);
                xs.Serialize(tw, this);
                tw.Close();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return false;
        }

        public ClixSenseAdProcessorConfig Load(string path)
        {
            if (!File.Exists(path)) return null;
            try
            {
                XmlSerializer xs = new XmlSerializer(typeof(ClixSenseAdProcessorConfig));
                using (var sr = new StreamReader(path))
                {
                    var clixSenseAdProcessorConfig = (ClixSenseAdProcessorConfig)xs.Deserialize(sr);
                    Logger.Info($"de-Serialized: {clixSenseAdProcessorConfig}");
                    
                    return clixSenseAdProcessorConfig;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                File.Delete(path);
            }
            return null;
        }
        public override string ToString()
        {
            return (this.ReportAllProperties());
        }


    }

    public class ClixSenseAdProcessor : PtcAdProcessor
    {
        //private List<string> words = commaWords.Split(',');

        public ClixSenseAdProcessorConfig clixConfig { get; set; }

        private readonly Dictionary<string,Bitmap> catBitmaps;

        private readonly List<InstalledVoice> installedVoices;

        private readonly SpeechRecognitionEngine speechRecognitionEngine = new SpeechRecognitionEngine();

        private readonly SpeechSynthesizer synthesizer = new SpeechSynthesizer();

        private List<string> cachedSounds;

        private readonly int zoomFactorX;

        private readonly int zoomFactorY;
   
        public ClixSenseAdProcessor(
            ScreenShotsManager screenShotsManager,
            List<Bitmap> toFindBitmaps,
            Dictionary<string, Bitmap> catBitmaps,
            List<Bitmap> closeBitmaps,
            int displayIndex,
            int zoomFactorX,
            int zoomFactorY,
            bool fakeMoves,
            Dispatcher cd)
        {
            this.screenShotsManager = screenShotsManager;
            this.toFindBitmaps = toFindBitmaps;
            this.closeBitmaps = closeBitmaps;
            this.catBitmaps = catBitmaps;
            this.fakeMoves = fakeMoves;
            this.zoomFactorX = zoomFactorX;
            this.zoomFactorY = zoomFactorY;
            this.CaptureCatImage = false;
            this.StopAllProcessing = false;
            this.DisplayScreen = DisplayIndex.GetDisplayRectangle(displayIndex);
            this.clixConfig = new ClixSenseAdProcessorConfig();
            if ((this.clixConfig = this.clixConfig.Load($".\\ClixSenseAdProcessorConfig" + ".xml")) == null)
            {
                this.clixConfig = new ClixSenseAdProcessorConfig();
                this.clixConfig.Save($".\\ClixSenseAdProcessorConfig" + ".xml");
                Logger.Info($"Saved {nameof(this.clixConfig)} correctly : {this.clixConfig}");
            }
            else
            {
                Logger.Info($"Loaded {nameof(this.clixConfig)} correctly : {this.clixConfig}");
            }

            this.currentDispatcher = cd;
            //this.TalkingCool = new Talking();
            Logger.Info($"ClickSense initialize - Trying to initialize speech ");
            try
            {
                this.installedVoices = new List<InstalledVoice>(this.synthesizer.GetInstalledVoices());
                this.synthesizer.Volume = 100; // 0...100
                this.synthesizer.Rate = -2; // -10...10
                this.synthesizer.SelectVoice(this.installedVoices.First().VoiceInfo.Name);
                var numbers = new Choices();
                numbers.Add("one", "two", "three", "four", "five", "Refresh", "Close"); //, "Close"
                // Create a GrammarBuilder object and append the Choices object.
                var gb = new GrammarBuilder();
                gb.Append(new GrammarBuilder(numbers), 1, 5);
                var grammer = new Grammar(gb) { Priority = 127 };
                this.speechRecognitionEngine.RequestRecognizerUpdate();
                this.speechRecognitionEngine.LoadGrammar(grammer);
                this.speechRecognitionEngine.SetInputToDefaultAudioDevice();
                this.speechRecognitionEngine.SpeechRecognized += this.SpeechRecognitionEngine_SpeechRecognized;
                this.speechRecognitionEngine.SpeechDetected += this.SpeechRecognitionEngine_SpeechDetected;
                this.speechRecognitionEngine.SpeechRecognitionRejected += this.SpeechRecognitionEngine_SpeechRecognitionRejected;
                this.speechRecognitionEngine.RecognizeCompleted += this.SpeechRecognitionEngine_RecognizeCompleted;
            }
            catch (Exception exception)
            {
                Logger.Error(exception.Message + exception.InnerException?.Message);
                throw exception;
            }
            Logger.Info($" Finished audio Initializer with no exceptions - found {this.installedVoices.Count} voices");
        }

        public bool CaptureCatImage { get; set; }

        private void PlaySimpleSound()
        {
            var audioPlayer = new AudioPlayer();
            this.cachedSounds = audioPlayer.LoadAllAudioFromDirectory(".\\Resources\\");
            var idx = this.rand.Next(0, this.cachedSounds.Count);
            Logger.Info($" Playing the audio file {this.cachedSounds[idx]} ");
            audioPlayer.Play(this.cachedSounds[idx]);
            audioPlayer.CloseWaveOut();
        }

        public override bool FindAd()
        {
           
            if (this.StopAllProcessing) return false;
            Thread.Sleep(1000);
            this.ShowSplashMessage(this.DisplayScreen.Location, "Finding Ad", 75000);
            if (!this.FoundAdAtPoint()) return false;

            if (!this.SolveTheCatCaptha()) return false;

            if (!this.WaitForCloseAndCloseIt()) return false;
            this.CloseSplash();
            return true;
        }

        private bool FoundAdAtPoint()
        {
            this.UpdateSplashMessage($"Found {this.toFindBitmaps.Count} to match with.");
            Point? foundBmpAt = null;
            var countdown = this.toFindBitmaps.Count;
            while (countdown-- > 0)
            {
                var bmpScreenBitmap = this.screenShotsManager.ScreenShot(this.DisplayScreen.X, this.DisplayScreen.Y, this.DisplayScreen.Width, this.DisplayScreen.Height, "sensea.bmp");
                Thread.Sleep(1000);
                this.UpdateSplashMessage($"Finding Ad ....{countdown}");
                foreach (var toFindBitmap in this.toFindBitmaps)
                {
                    foundBmpAt = this.screenShotsManager.Find(bmpScreenBitmap, toFindBitmap);
                    if (foundBmpAt.HasValue) break;
                }
                if (foundBmpAt.HasValue) break;
            }
            if (foundBmpAt != null)
            {
                this.UpdateSplashMessage($"Clicking the found ad {foundBmpAt}");
                this.ClickTheFoundAd(foundBmpAt);
            }
            else
            {
                this.UpdateSplashMessage($"Ad Not Found returning false");
                return false;
            }
            return true;
        }


        private bool WaitForCloseAndCloseIt()
        {
            var counter = 0;
            Point? foundCloseAt;
            while (true)
            {
                if (this.StopAllProcessing) return false;
               this.UpdateSplashMessage($" Finding the Close button");
                foundCloseAt = this.FindSenseClose();
                Thread.Sleep(1000);
                if (foundCloseAt.HasValue || counter++ > 60)
                {
                    MouseInput.MoveMouse(
                        this.DisplayScreen.X + this.DisplayScreen.Width / 2 + this.rand.Next(50, 100),
                        this.DisplayScreen.Y + this.rand.Next(0, 50));
                    this.mouseSimulator.LeftButtonClick();
                    this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
                    break;
                }
            }
         
            if (foundCloseAt != null)
                this.UpdateSplashMessage($"Found Close after : {counter}");
          
            return true;
        }

        private bool SolveTheCatCaptha()
        {
            this.CaptureCatImage = true;
            var catSearchDone = false;
            var recognizeVoiceDone = false;
            this.UpdateSplashMessage( "Finding the Cat ...");
            bool catDbSearchThreadActive = false;
            Thread catDbSearchThread = null;
            Bitmap bmpScreenBitmap2 = null;
            while (this.CaptureCatImage)
            {
                Thread.Sleep(500);
                if (this.StopAllProcessing) return false;
                if (catDbSearchThreadActive == false)
                {
                    catDbSearchThread = new Thread(
                        () =>
                            {
                                if (!catSearchDone)
                                {
                                    if (bmpScreenBitmap2 == null)
                                    {
                                       
                                        bmpScreenBitmap2 = this.screenShotsManager.ScreenShot(
                                            this.DisplayScreen.X,
                                            this.DisplayScreen.Y,
                                            this.clixConfig.CatCapthaBottomRight.X,
                                            this.clixConfig.CatCapthaBottomRight.Y,
                                            "senseb.bmp");
                                    }
                                    var foundCat = this.FindTheCat((Bitmap)bmpScreenBitmap2);
                                    if (foundCat.HasValue)
                                    {
                                        this.CaptureCatImage = false;
                                        SmoothMouseMove.MoveMouse(
                                            this.DisplayScreen.X + foundCat.Value.X + this.rand.Next(0, 20),
                                            this.DisplayScreen.Y + foundCat.Value.Y + this.rand.Next(0, 14),
                                            2,
                                            2);
                                        this.UpdateSplashMessage("Found the Cat in DB Woo... Hoo...");
                                        Thread.Sleep(5000);
                                        this.mouseSimulator.LeftButtonClick();
                                    }
                                    catSearchDone = true;
                                }
                            }) { Name = "CatDbSearchThread" };
                    catDbSearchThread.Start();
                    catDbSearchThreadActive = true;
                }

                if (recognizeVoiceDone == false)
                {
                    recognizeVoiceDone = true;
                    this.Say();
                }
                this.UpdateSplashMessage($"Waiting for user to select the cat ");
            }
            catDbSearchThread?.Abort();
            return true;
        }

        private void ClickTheFoundAd(Point? foundBmpAt)
        {
            var processHandle = ProcessManager.GetProcessHandle("firefox");
            if (foundBmpAt != null)
            {
                var pointToScreen = new Point(
                    this.DisplayScreen.X + foundBmpAt.Value.X + this.rand.Next(0, this.clixConfig.AdClickXRange),
                    this.DisplayScreen.Y + foundBmpAt.Value.Y + this.rand.Next(0, this.clixConfig.AdClickYRange));
                SmoothMouseMove.MoveMouse(pointToScreen.X, pointToScreen.Y, 1, 1);
            }
            this.mouseSimulator.LeftButtonClick();
            Thread.Sleep(500);
        }

     
        public override bool GamePrize()
        {
            Logger.Error($"{MethodBase.GetCurrentMethod().Name}");
            return true;
        }

        public bool GamePrize(Bitmap endGameBitmap)
        {
            if (this.StopAllProcessing) return false;
            var stopwatch = new Stopwatch();
            var processedItems = new List<Tuple<int, int>>();
            stopwatch.Start();
            var waitForClose = false;
            var count = 0;
            var gridCellWidth =  Math.Abs(this.clixConfig.ClixGridTopLeft.X - this.clixConfig.ClixGridBottomRight.X) / this.clixConfig.ClixGridXDivisions; 
            var gridCellHeight = Math.Abs(this.clixConfig.ClixGridTopLeft.Y - this.clixConfig.ClixGridBottomRight.Y) / this.clixConfig.ClixGridYDivisions; 
            var row = 0;
            var column = 0;
            while (true)
            {
                if (this.StopAllProcessing) return false;
                Thread.Sleep(this.rand.Next(500, 1000)); // Configuration
                if (waitForClose == false)
                {
                    var skipPoint = false;
                    this.ShowSplashMessage(this.DisplayScreen.Location, "Calculating next click point", 30000);
                  
                    // column = this.rand.Next(1, 30);
                    this.UpdateSplashMessage($" {nameof(this.clixConfig.ClixGridTopLeft.X)} : {this.clixConfig.ClixGridTopLeft.X} ,  {nameof(this.clixConfig.ClixGridTopLeft.Y)} : {this.clixConfig.ClixGridTopLeft.Y}");
                    var clickPoint = new Point(this.clixConfig.ClixGridTopLeft.X + column * gridCellWidth + gridCellWidth/2 + column, this.clixConfig.ClixGridTopLeft.Y + row * gridCellHeight + gridCellHeight/2 + row);
                  //  clickPoint.Offset(-gridCellWidth, -gridCellHeight);
                    this.UpdateSplashMessage($" {nameof(row)} : {row} [{gridCellWidth} ,  {nameof(column)} : {column} [{gridCellHeight}] , X= {clickPoint.X}, Y= {clickPoint.Y}");
                    foreach (var item in processedItems)
                    {
                        if (item.Item1 == row && item.Item2 == column)
                        {
                             skipPoint = true;
                            this.UpdateSplashMessage("Skipping point as it was already used");
                            break;
                        }
                    }
                    if (skipPoint) continue;
                    processedItems.Add(new Tuple<int, int>(row,column));
                    SmoothMouseMove.MoveMouse(clickPoint);
                    Thread.Sleep(this.rand.Next(1000, 2000)); // Configuration
                    this.mouseSimulator.LeftButtonClick();
                    waitForClose = true;
                    if (row++ == 19)
                    {
                        row = 0;
                        column++;
                        continue;
                    }
                }
                else
                {
                    var bmpScreenBitmap = this.screenShotsManager.ScreenShot("clixgrid.bmp");
                    Thread.Sleep(1000);
                    Point? foundBmpAt = null;
                    this.UpdateSplashMessage("Looking for close");
                    foreach (var toCloeBitmap in this.closeBitmaps)
                    {
                        foundBmpAt = this.screenShotsManager.Find(bmpScreenBitmap, toCloeBitmap);
                        if (foundBmpAt.HasValue) break;
                    }
                    if (foundBmpAt != null)
                    {
                        this.Close();
                        this.UpdateSplashMessage($" Close found after {stopwatch.ElapsedMilliseconds / 1000} seconds.");
                        count = 0;
                        stopwatch.Stop();
                        stopwatch.Restart();
                        Thread.Sleep(3000);
                        waitForClose = false;
                    }
                }
                if (endGameBitmap != null)
                {
                    var bmpEndGame = this.screenShotsManager.ScreenShot("clixgridend.bmp");
                    var result = this.screenShotsManager.Find(bmpEndGame, endGameBitmap);
                    if (result.HasValue)
                    {
                        this.Close();
                        return true;
                    }
                }
                if (count == 35) // Configuration Refresh After x tries
                {
                    this.Refresh();
                    waitForClose = false;
                }

                if (count++ > 60) // Configuration Close After x tries
                {
                    this.Close();
                    return true;
                }
            
                this.UpdateSplashMessage($"Close NOT found after {stopwatch.ElapsedMilliseconds / 1000} seconds.");
              
            }
          
        }

        public void SaveSenseCatImageToDb(MouseEventArgs e)
        {
            try
            {
                if (this.CaptureCatImage)
                {
                    var catImage = $".\\Cats\\{this.catBitmaps.Count + 1}.bmp";
                    Directory.CreateDirectory(".\\CompareCats\\");
                    var compareImage = $".\\CompareCats\\{this.catBitmaps.Count + 1}.bmp";
                    Logger.Info($"catImage:{catImage} , CompareImage: {compareImage}");

                    var temp = this.screenShotsManager.ScreenShot(e.X, e.Y, 8, 8, catImage);
                    Thread.Sleep(500);
                    var compare = this.screenShotsManager.ScreenShot(this.DisplayScreen.X, this.DisplayScreen.Y , this.clixConfig.CatCapthaBottomRight.X, this.clixConfig.CatCapthaBottomRight.Y, compareImage);
                    Logger.Info($" Captured cat is found in Target? : {this.screenShotsManager.Find(compare, temp)}");
                    this.catBitmaps.Add(catImage,temp);
                }
            }
            catch (Exception exception)
            {
                this.UpdateSplashMessage("SaveSenseCatImageToDb" + exception.Message + exception.InnerException?.Message);
            }
        }

        public Point? FindTheCat(Bitmap screenShot)
        {
            // return null;
            Point? foundCatPoint = null;
            Rectangle catRectangle = new Rectangle(this.clixConfig.CatCapthaTopLeft, 
                new Size(this.clixConfig.CatCapthaBottomRight.X - this.clixConfig.CatCapthaTopLeft.X,
                        this.clixConfig.CatCapthaBottomRight.Y - this.clixConfig.CatCapthaTopLeft.Y));
            Logger.Info($"Cat Rectangle : {catRectangle}");

            try
            {
                var keyValuePairs = this.catBitmaps.ToList();
                for (int index = keyValuePairs.Count; index > 0; --index)
                {
                    var catBitmap = keyValuePairs[index - 1];
                    if (this.StopAllProcessing) return null;
                  
                    foundCatPoint = this.screenShotsManager.Find(screenShot, catBitmap.Value);
                    if (foundCatPoint != null && !catRectangle.Contains(foundCatPoint.Value))
                    {
                        this.UpdateSplashMessage( $"Found a invalid Point Delete Image ? : { catBitmap.Key},  { foundCatPoint}");
                        Thread.Sleep(1000);
                          catBitmap.Value.Dispose();
                        // var toDeleteLocation = Path.Combine(Directory.GetCurrentDirectory(), "ToDelete", catBitmap.Key);
                           File.Delete(catBitmap.Key);
                        continue;
                    }
                    if (foundCatPoint.HasValue)
                    {
                        this.UpdateSplashMessage($" Found a matching Cat: {catBitmap.Key} {foundCatPoint}");
                        Thread.Sleep(1000);
                        break;
                    }
                    this.UpdateSplashMessage($"looking through cats DB of : { keyValuePairs.Count} : {index}");
                }
            }
            catch (Exception e)
            {
                this.UpdateSplashMessage(e.Message + e.InnerException?.Message);
                Thread.Sleep(2000);
            }
            this.UpdateSplashMessage($"NO matching Cat found - returning null ");
            return foundCatPoint;
        }

        private Point? FindSenseClose()
        {
            var bmpScreenBitmap3 = this.screenShotsManager.ScreenShot(this.DisplayScreen.X, this.DisplayScreen.Y, this.DisplayScreen.Width, this.DisplayScreen.Height, "sensec.bmp");
            Point? foundCloseAt = Point.Empty;
            foreach (var close in this.closeBitmaps)
            {
                foundCloseAt = this.screenShotsManager.Find(bmpScreenBitmap3, close);
                if (foundCloseAt.HasValue) break;
            }
            return foundCloseAt;
        }

        private void Say()
        {
           
            this.synthesizer.Speak("Select the Cat");
            this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        private void SpeechRecognitionEngine_RecognizeCompleted(object sender, RecognizeCompletedEventArgs e)
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Done with recognition ...");
            if (this.CaptureCatImage) this.speechRecognitionEngine.RecognizeAsync(RecognizeMode.Single);
        }

        private void SpeechRecognitionEngine_SpeechRecognitionRejected(object sender, SpeechRecognitionRejectedEventArgs e)
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name} - {e.Result.Text} ");
            this.UpdateSplashMessage($" Rejected Speech {e.Result.Text}");
            // this.SelectACat(e.Result.Text);
        }

        private void SpeechRecognitionEngine_SpeechDetected(object sender, SpeechDetectedEventArgs e)
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name} - - {e.AudioPosition.TotalMilliseconds}");
        }

        private void SpeechRecognitionEngine_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            Logger.Info($" speech recognised : {e.Result.Text}");
            this.UpdateSplashMessage($" Speech recognised {e.Result.Text}");
            this.SelectACat(e.Result.Text);
        }

        private void SelectACat(string text)
        {
          
            if (string.IsNullOrEmpty(text)) return;
            var offset = this.rand.Next(5, 20);
            switch (text)
            {
                case "one":
                case "1":
                case "number one":
                    this.CatRecognised(new Point(80 + offset, 250));
                    break;
                case "number two":
                case "two":
                case "2":
                    this.CatRecognised(new Point(210 + offset, 250));
                    break;
                case "number three":
                case "three":
                case "3":
                    this.CatRecognised(new Point(350 + offset, 250));
                    break;
                case "number four":
                case "four":
                case "4":
                    this.CatRecognised(new Point(500 + offset, 250));
                    break;
                case "number five":
                case "five":
                case "5":
                    this.CatRecognised(new Point(620 + offset, 250));
                    break;
                case "Refresh":
                    this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.F5);
                    break;
                case "Close":
                    this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
                    this.CaptureCatImage = false;
                    break;
            }
        }

        private void CatRecognised(Point pt)
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name}, zoomFactor: {this.zoomFactorX}, {this.zoomFactorY}");
            var xpt = pt.X * (1 + (float)this.zoomFactorX / 100) + this.rand.Next(-30, 30);
            var ypt = pt.Y * (1 + (float)this.zoomFactorY / 100) + this.rand.Next(-30, 30);
            SmoothMouseMove.MoveMouse(this.DisplayScreen.X + (int)xpt, this.DisplayScreen.Y + (int)ypt, 1, 1);
            this.mouseSimulator.LeftButtonClick();
            this.CaptureCatImage = false;
            this.speechRecognitionEngine.RecognizeAsyncStop();
        }
    }
}