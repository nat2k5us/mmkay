namespace PtcAdProcessor
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Linq;
    using System.Threading;

    using WindowsInput.Native;

    using KeyboardMouse;

    using ProcessManagement;

    using ScreenShots;
    using System;

    public class MegaAdProcessor : PtcAdProcessor
    {
        public MegaAdProcessor(
            ScreenShotsManager screenShotsManager,
            List<Bitmap> toFindBitmaps,
            List<Bitmap> closeBitmaps,
            bool fakeMoves)
        {
            this.screenShotsManager = screenShotsManager;
            this.toFindBitmaps = toFindBitmaps;
            this.closeBitmaps = closeBitmaps;
            this.fakeMoves = fakeMoves;
            this.StopAllProcessing = false;
        }


        public override bool FindAd()
        {
            if (this.StopAllProcessing) return false;
            Thread.Sleep(3000); // wait for few seconds before 
            var bmpScreenBitmap = this.screenShotsManager.ScreenShot("shota.bmp");
            // this.BackgroundImage = bmpScreenBitmap;
            Point? foundBmpAt = Point.Empty;
            foreach (var toFindBitmap in this.toFindBitmaps)
            {
                foundBmpAt = this.screenShotsManager.Find(bmpScreenBitmap, toFindBitmap);
                if (foundBmpAt.HasValue) break;
            }
           
            if (foundBmpAt.HasValue)
            {
                Logger.Info($"Found an advert");
                SmoothMouseMove.MoveMouse(
                    foundBmpAt.Value.X + this.rand.Next(10, 40),
                    foundBmpAt.Value.Y + this.rand.Next(20, 25),
                    2,
                    1);
                Thread.Sleep(2500);
                this.mouseSimulator.LeftButtonClick();
            
                Point? foundCaptcha = null;
                var counter = 0;
                do
                {
                    if (this.StopAllProcessing) return false;
                    counter++;
                    Thread.Sleep(1000);
                    SmoothMouseMove.MoveMouseInCircle(this.rand.Next(5, 200));
                    //this.keyboardSimulator.TextEntry();
                    SmoothMouseMove.MoveMouse(this.rand.Next(this.DisplayScreen.X, this.DisplayScreen.Width), this.rand.Next(this.DisplayScreen.Y, this.DisplayScreen.Height), 2, 2);
                    SmoothMouseMove.MoveMouseInCircle(this.rand.Next(50, 100));

                    var webBrowserTabTitle = BrowserProcessHelpers.GetWebBrowserTabTitle("firefox");
                    if (webBrowserTabTitle.Contains("Internal Server Error"))
                    {
                        this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
                        Logger.Info($" Keyboard Close");
                        return true;
                    }
                }
                while (!this.MegaAdCaptchaSolve(out foundCaptcha) && counter < 7);

                if (foundCaptcha.HasValue)
                {
                    SmoothMouseMove.MoveMouse(foundCaptcha.Value.X + this.rand.Next(10, 40),foundCaptcha.Value.Y + this.rand.Next(10, 24),2,2);
                    Thread.Sleep(50);
                    this.mouseSimulator.LeftButtonClick();
                    Thread.Sleep(1000);
                    this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
                    Logger.Info($" Keyboard Close");
                }
            }
            else
            {
                Logger.Info($"No Ad Found ...");
                return false;
            }
            return true;
        }

        public override bool GamePrize()
        {
            throw new NotImplementedException();
        }

        private bool FindMegaClose()
        {
            ////  this.buttonFindNemo.Focus();
            //var bmpScreenBitmap3 = this.screenShotsManager.ScreenShot("shotc.bmp");
            //var foundCloseAt = null; // ImageProcessing.FindBitmapsEntry(bmpScreenBitmap3, Resources.megaclose, true).First();

           
            //if (!foundCloseAt.IsEmpty)
            //{
            //    this.keyboardSimulator.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_W);
            //    Logger.Info($" Keyboard Close");

            //    return true;
            //}
            return false;
        }

        public bool MegaAdCaptchaSolve(out Point? point)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var bmpScreenBitmap3 = this.screenShotsManager.ScreenShot("shotc.bmp");
            List<Point> foundCloseAt = null;
            foreach (var closeBitmap in this.closeBitmaps)
            {
                foundCloseAt = ImageProcessing.FindBitmapsEntry(bmpScreenBitmap3, closeBitmap, true);
                if (foundCloseAt.Any()) break;
            }
            
            stopwatch.Stop();
            Logger.Info($"Time in Captcha {stopwatch.ElapsedMilliseconds} milliseconds : Solved? : {foundCloseAt}");
            point = foundCloseAt?.First();
            return (foundCloseAt != null);
        }
    }
}