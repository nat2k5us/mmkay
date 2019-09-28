namespace ScreenShotsTests
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using ScreenShots;

    [TestClass]
    public class ScreenShotsManagerTests
    {
        [TestMethod]
    
        public void FindNemoCloseTest()
        {
            var shotcBmp = @"shotc.bmp";
            var nemoCloseBmp = @"1.bmp";

            var directoryInfo = Directory.GetParent(Directory.GetCurrentDirectory()).Parent;
            if (directoryInfo != null) Console.WriteLine(directoryInfo.FullName);
            var hayStackShotPath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\FindImageInImage\\bin\\Debug", shotcBmp);
            var needlePath = Path.Combine(Directory.GetCurrentDirectory(), "..\\..\\..\\FindImageInImage\\bin\\Debug\\NemoCloses", nemoCloseBmp);
            Console.WriteLine(File.Exists(hayStackShotPath) ? $"  {hayStackShotPath} Exists" : $" {hayStackShotPath} does not Exist");
            Console.WriteLine(File.Exists(needlePath) ? $"  {needlePath} Exists" : $" {needlePath} does not Exist");
            var haystackBitmap = (Bitmap)Image.FromFile(hayStackShotPath);
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(needlePath);
            Assert.IsNotNull(needleBitmap);
            var screenShotsManager = new ScreenShotsManager();
            var result = screenShotsManager.Find(haystackBitmap, needleBitmap);
            Assert.IsNotNull(result);
            Console.WriteLine($"Needle in Haystack Found? {result}");
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\testStrip2.bmp")]
        [DeploymentItem(@".\Resources\eyepop.bmp")]
        public void FindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@"testStrip2.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\eyepop.bmp");
            Assert.IsNotNull(needleBitmap);
            var screenShotsManager = new ScreenShotsManager();
            var result = screenShotsManager.Find(haystackBitmap, needleBitmap);
            Console.WriteLine(result.ToString());
        }

        [TestMethod]
        public void LoadAllBitmapsFromDirectoryDictionaryTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var shotsManager = new ScreenShotsManager();
            var bitmaps = shotsManager.LoadAllBitmapsFromDirectoryAsDictionary(".\\Cats\\");
            stopwatch.Stop();
            Console.WriteLine($"LoadAllBitmapsFromDirectory took {stopwatch.ElapsedMilliseconds} milliseconds for {bitmaps.Count} files");
        }

        [TestMethod]
        public void LoadAllBitmapsFromDirectoryTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var shotsManager = new ScreenShotsManager();
            var bitmaps = shotsManager.LoadAllBitmapsFromDirectory(".\\Cats\\");
            stopwatch.Stop();
            Console.WriteLine($"LoadAllBitmapsFromDirectory took {stopwatch.ElapsedMilliseconds} milliseconds for {bitmaps.Count} files");
        }

        [TestMethod]
        public void RenameBitmapsFromDirectoryTest()
        {
            var shotsManager = new ScreenShotsManager();
            shotsManager.RenameBitmapsFromDirectory(@"F:\Development\WPF\MyDream\FindImageInImage\bin\Debug\Cats");
        }

        [TestMethod]
        public void SaveAllBitmapsFromDirectoryTest()
        {
            var shotsManager = new ScreenShotsManager();
            shotsManager.SaveAllBitmapsFromDirectory(@"F:\Development\WPF\MyDream\FindImageInImage\bin\Debug\Cats");
        }

        [TestMethod]
        public void ScreenShotTest()
        {
            var screenShotsManager = new ScreenShotsManager();
            Assert.IsNotNull(screenShotsManager, "ScreenShotManager could not be initialized");
            screenShotsManager.ScreenShot(-1920, 0, 1920, 1080, "../../test.bmp");
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\ptc1shot2.bmp")]
        [DeploymentItem(@".\Resources\ptc1ad.bmp")]
        public void SearchBitmapPtc1Ad2FindFailTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\ptc1shot2.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\ptc1ad.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel took {stopwatch.ElapsedMilliseconds} milliseconds - {pt}");
            Assert.IsTrue(!pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\ptc1adsshot.bmp")]
        [DeploymentItem(@".\Resources\ptc1ad.bmp")]
        public void SearchBitmapPtc1AdFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\ptc1adsshot.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\ptc1ad.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel took {stopwatch.ElapsedMilliseconds} milliseconds - {pt}");
            Assert.IsTrue(pt.HasValue);
        }

        // ptc1shot
        [TestMethod]
        [DeploymentItem(@".\Resources\ptc1shot.bmp")]
        [DeploymentItem(@".\Resources\ptc1solve.bmp")]
        public void SearchBitmapPtc1Test()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\ptc1shot.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\ptc1solve.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel took {stopwatch.ElapsedMilliseconds} milliseconds - {pt}");
            Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\sensecloseshot.bmp")]
        [DeploymentItem(@".\Resources\senseclose.bmp")]
        public void SearchBitmapSenseCloseFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\sensecloseshot.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\senseclose.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel took {stopwatch.ElapsedMilliseconds} milliseconds - {pt}");
            Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\testStrip3.bmp")]
        [DeploymentItem(@".\Resources\eyepop.bmp")]
        public void SearchBitmapTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\testStrip3.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\eyepop.bmp");
            Assert.IsNotNull(needleBitmap);

            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\testStrip2.bmp")]
        [DeploymentItem(@".\Resources\eyepop.bmp")]
        public void SearchBitmapTest2()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\testStrip2.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\eyepop.bmp");
            Assert.IsNotNull(needleBitmap);

            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\testStrip.bmp")]
        [DeploymentItem(@".\Resources\mario.bmp")]
        public void SearchBitmapTest3()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\testStrip.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\mario.bmp");
            Assert.IsNotNull(needleBitmap);

            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\megabuxtest.bmp")]
        [DeploymentItem(@".\Resources\megabux.bmp")]
        public void SearchBitmapTestMegaBuxAd()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\megabuxtest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\megabux.bmp");
            Assert.IsNotNull(needleBitmap);

            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\shotbusty.bmp")]
        [DeploymentItem(@".\Resources\busty.bmp")]
        public void SearchBitmapTestMegaBuxBustyFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\shotbusty.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\busty.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\megaclosetest.bmp")]
        [DeploymentItem(@".\Resources\megaclose.bmp")]
        public void SearchBitmapTestMegaBuxCloseTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\megaclosetest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\megaclose.bmp");
            Assert.IsNotNull(needleBitmap);

            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\DonkeyTest.bmp")]
        [DeploymentItem(@".\Resources\donkey.bmp")]
        public void SearchBitmapTestMegaBuxDonkeyFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\DonkeyTest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\donkey.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\ghosttest.bmp")]
        [DeploymentItem(@".\Resources\ghost.bmp")]
        public void SearchBitmapTestMegaBuxGhostFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\ghosttest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\ghost.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\InvalidShot.bmp")]
        [DeploymentItem(@".\Resources\Invalid.bmp")]
        public void SearchBitmapTestMegaBuxInvalidFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\InvalidShot.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\Invalid.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\RobotTest.bmp")]
        [DeploymentItem(@".\Resources\robo.bmp")]
        public void SearchBitmapTestMegaBuxRoboFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\RobotTest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\robo.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\SheepTest.bmp")]
        [DeploymentItem(@".\Resources\Sheep.bmp")]
        public void SearchBitmapTestMegaBuxSheepFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\SheepTest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\Sheep.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\loadfailedshot.bmp")]
        [DeploymentItem(@".\Resources\failedLoad.bmp")]
        public void SearchBitmapTestMegaFailedToloadImageTest()
        {
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\loadfailedshot.bmp");
            Assert.IsNotNull(haystackBitmap);
            var needleBitmap = (Bitmap)Image.FromFile(@".\failedLoad.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
            stopwatch.Start();

            // var shotsManager = new ScreenShotsManager();
            // var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            // stopwatch.Stop();
            // Console.WriteLine($"Image Find GetPixel way Took {stopwatch.ElapsedMilliseconds} milliseconds");
            // Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\serverErrorShot.bmp")]
        [DeploymentItem(@".\Resources\serverError.bmp")]
        public void SearchBitmapTestMegaServerErrorFindTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\serverErrorShot.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\serverError.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var points = ImageProcessing.FindBitmapsEntry(haystackBitmap, needleBitmap, true);
            stopwatch.Stop();
            Console.WriteLine($"Image Find Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(points.Count > 0);
            points.ForEach(x => Console.WriteLine(x.ToString()));
            stopwatch.Start();
            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel way Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\megabuxtest.bmp")]
        [DeploymentItem(@".\Resources\megabux.bmp")]
        public void SearchBitmapTestMegFindByPixelTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\megabuxtest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\megabux.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel way Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\pindadashot.bmp")]
        [DeploymentItem(@".\Resources\neopinkad.bmp")]
        public void SearchBitmapTestNeoPinkAdTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\pindadashot.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\neopinkad.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel way Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        [DeploymentItem(@".\Resources\neofindAdTest.bmp")]
        [DeploymentItem(@".\Resources\ToFind.bmp")]
        public void SearchBitmapTestNeoUsingFindByPixelTest()
        {
            // string solution_dir = Path.GetDirectoryName(Path.GetDirectoryName(TestContext.TestDir));
            Console.WriteLine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName);
            var haystackBitmap = (Bitmap)Image.FromFile(@".\neofindAdTest.bmp");
            Assert.IsNotNull(haystackBitmap);

            var needleBitmap = (Bitmap)Image.FromFile(@".\ToFind.bmp");
            Assert.IsNotNull(needleBitmap);
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var shotsManager = new ScreenShotsManager();
            var pt = shotsManager.Find(haystackBitmap, needleBitmap);
            stopwatch.Stop();
            Console.WriteLine($"Image Find GetPixel way Took {stopwatch.ElapsedMilliseconds} milliseconds");
            Assert.IsTrue(pt.HasValue);
        }

        [TestMethod]
        public void TakeScreenShotTest()
        {
            var screenShotsManager = new ScreenShotsManager();
            Assert.IsNotNull(screenShotsManager, "ScreenShotManager could not be initialized");
            screenShotsManager.TakeScreenShot();
        }

        [TestMethod]
        public void TempTest()
        {
            try
            {
                var screenShotsManager = new ScreenShotsManager();
                Assert.IsNotNull(screenShotsManager, "ScreenShotManager could not be initialized");
                screenShotsManager.TakeScreenShot();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.InnerException?.Message);
            }
        }
    }
}