using System.Collections.Generic;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using System.Windows.Threading;

    using KeyboardMouse;

    using ScreenShots;
namespace PtcAdProcessor
{
   

    public class NemoAdProcessor : PtcAdProcessor
    {
        private readonly List<Bitmap> clickDotBitmap;
        public NemoAdProcessor(
            ScreenShotsManager screenShotsManager,
            List<Bitmap> toFindBitmaps,
            List<Bitmap> clickDotBitmap,
            List<Bitmap> closeBitmaps,
            int displayIndex,
            bool fakeMoves,
            Dispatcher cd)
        {
            this.screenShotsManager = screenShotsManager;
            this.toFindBitmaps = toFindBitmaps;
            this.clickDotBitmap = clickDotBitmap;
            this.closeBitmaps = closeBitmaps;
            this.fakeMoves = fakeMoves;
            this.DisplayScreen = DisplayIndex.GetDisplayRectangle(displayIndex);
            this.StopAllProcessing = false;
            this.currentDispatcher = cd;
            Logger.Info($"{MethodBase.GetCurrentMethod().Name} - Finished Initialization");
        }

        public override bool FindAd()
        {
            Logger.Info($"{MethodBase.GetCurrentMethod().Name}");
            if (this.StopAllProcessing) return false;
            Thread.Sleep(this.rand.Next(1000,3000));
            
            if (!this.FindTheAdAndClickIt()) return false;
            if (!this.FindTheDotAndClickIt()) return false;
            if (!this.LookForClose()) return false;

            Logger.Info($"Finished with the Ad ... Moving to the next");

            return true;
        }

        private bool FindTheAdAndClickIt()
        {
            if (this.StopAllProcessing) return false;
            var bmpScreenBitmap = this.screenShotsManager.ScreenShot("shota.bmp");
            Point? foundBmpAt = Point.Empty;
            this.ShowSplashMessage(this.DisplayScreen.Location, "Finding Ad", 40000);
            for (int index = 0; index < this.toFindBitmaps.Count; index++)
            {
                var toFindBitmap = this.toFindBitmaps[index];
                foundBmpAt = this.screenShotsManager.Find(bmpScreenBitmap, toFindBitmap);
                if (foundBmpAt.HasValue)
                {
                    if (foundBmpAt.Value.Y > 800)
                    {
                        SmoothMouseMove.MoveMouse(foundBmpAt.Value.X, foundBmpAt.Value.Y, 1, 1);
                        MouseInput.ScrollWheel(-1);
                        Thread.Sleep(5000);
                         bmpScreenBitmap = this.screenShotsManager.ScreenShot("shota.bmp");
                        index = 0;
                        continue;
                    }
                    break;
                }
            }

            if (foundBmpAt.HasValue)
            {
                this.UpdateSplashMessage($" Found an advert at : {foundBmpAt}");
                SmoothMouseMove.MoveMouseInCircle(5);
                SmoothMouseMove.MoveMouse(foundBmpAt.Value.X + this.rand.Next(-50, 50), foundBmpAt.Value.Y + this.rand.Next(-100, 100), 2, 1);
                SmoothMouseMove.MoveMouse(foundBmpAt.Value.X, foundBmpAt.Value.Y, 1, 1);
                Thread.Sleep(this.rand.Next(100, 1000));
                this.mouseSimulator?.LeftButtonClick();
            }
            else
            {
                this.UpdateSplashMessage($" No Ad Found ,returning false");
                return false;
            }
            return true;
        }

        private bool FindTheDotAndClickIt()
        {
            Point? foundDotAt = null;
            var count = 30;
            this.UpdateSplashMessage("Finding The dot");
            do
            {
                if (this.StopAllProcessing) return false;
                var bmpScreenBitmap2 = this.screenShotsManager.ScreenShot("shotb.bmp");
                Thread.Sleep(this.rand.Next(100, 1000));
                foreach (var dotBitmap in this.clickDotBitmap)
                {
                    this.UpdateSplashMessage($"Trying to find the dot {count--}");
                    foundDotAt = this.screenShotsManager.Find(bmpScreenBitmap2, dotBitmap);
                    if (foundDotAt.HasValue) break;
                }
            }
            while (foundDotAt == null && count > 0);

            if (foundDotAt != null)
            {
                Thread.Sleep(this.rand.Next(500, 2000));
                this.UpdateSplashMessage($"Found the dot {foundDotAt}");
                SmoothMouseMove.MoveMouse(foundDotAt.Value.X + this.rand.Next(-100, 100), foundDotAt.Value.Y + this.rand.Next(-100, 100), 2, 1);
                SmoothMouseMove.MoveMouse(foundDotAt.Value.X + this.rand.Next(1, 5), foundDotAt.Value.Y + this.rand.Next(-3, 5), 2, 2);
                Thread.Sleep(this.rand.Next(500, 2000));
                this.mouseSimulator?.LeftButtonClick();
            }
            else
            {
                this.UpdateSplashMessage( "Dot not found .. returning false");
                return false;
            }
            return true;
        }

        private bool LookForClose()
        {
            var count = 0;
            do
            {
                if (this.StopAllProcessing) return false;
                this.UpdateSplashMessage($" Looking for close {count}.");
                Thread.Sleep(1000);
                if (this.fakeMoves)
                {
                    SmoothMouseMove.MoveMouseInCircle(this.rand.Next(this.DisplayScreen.X, this.DisplayScreen.Width));
                    //this.keyboardSimulator.TextEntry();
                    SmoothMouseMove.MoveMouse(this.rand.Next(this.DisplayScreen.X, this.DisplayScreen.Width), this.rand.Next(this.DisplayScreen.Y, this.DisplayScreen.Height), 2, 2);
                    SmoothMouseMove.MoveMouseInCircle((this.rand.Next(this.DisplayScreen.X, this.DisplayScreen.Width)));
                }
               
                if (count == 25) this.Refresh();
              
                if (count++ > 30)
                {
                    this.UpdateSplashMessage($"Close not found after {count} trys");
                    this.Close();
                    return true;
                }
            }
            while (this.CloseAd() == false);
            this.CloseSplash();
            return true;
        }

        public override bool GamePrize()
        {
            if (this.StopAllProcessing) return false;
            int count = 0;
           
            while (true)
            {
               
                if (this.StopAllProcessing) return false;
                var bmpScreenBitmap = this.screenShotsManager.ScreenShot("adprize.bmp");
                Thread.Sleep(this.rand.Next(500, 1500));
                Point? foundBmpAt = null;
                this.UpdateSplashMessage($" Looking for Ad ...");
                foreach (var toFindBitmap in this.toFindBitmaps)
                {
                    foundBmpAt = this.screenShotsManager.Find(bmpScreenBitmap, toFindBitmap);
                    if (foundBmpAt.HasValue)
                    {
                        this.UpdateSplashMessage($" Found Ad ...");
                        break;
                    }
                }

                if (foundBmpAt != null)
                {
                   
                    SmoothMouseMove.MoveMouse(foundBmpAt.Value.X + this.rand.Next(5, 30), foundBmpAt.Value.Y + +this.rand.Next(2, 8), 2, 2);
                    Thread.Sleep(this.rand.Next(100, 500));
                    this.mouseSimulator?.LeftButtonClick();
                    MouseInput.MoveMouse(foundBmpAt.Value.X + this.rand.Next(50, 100), foundBmpAt.Value.Y + this.rand.Next(0, 280));
                    SmoothMouseMove.MoveMouseInCircle(50);
                    count = 0;
                    this.ShowSplashMessage(this.DisplayScreen.Location, "Finished clicking ...", 40000);
                    Thread.Sleep(this.rand.Next(100, 500));
                }
              
                if (count++ == 20)
                    this.Refresh();
               
                if (count > 30)
                {
                   this.Close();
                    return true;
                }
            }
            
        }

        public bool CloseAd()
        {
            if (this.StopAllProcessing) return false;
            var bmpScreenBitmap3 = this.screenShotsManager.ScreenShot("shotc.bmp");
            Point? foundCloseAt = null;
            Thread.Sleep(500);
            this.UpdateSplashMessage("Finding close");
            Logger.Info($"Looking for close in {this.closeBitmaps.Count} bitmaps");
            foreach (var closeBitmap in this.closeBitmaps)
            {
                foundCloseAt = this.screenShotsManager.Find(bmpScreenBitmap3, closeBitmap);
                if (foundCloseAt.HasValue) break;
            }
         
            Thread.Sleep(500);
            Logger.Info($"Close Found? : {foundCloseAt.HasValue} ");
            if (foundCloseAt.HasValue )
            {
               this.Close();
               return true;
            }
            Logger.Info($" Did not find the Close Bitmap");
            return false;
        }
    }
}