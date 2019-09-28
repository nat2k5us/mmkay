namespace ScreenShots
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    using NLog;

    public class ScreenShotsManager
    {
        // ImageConverter object used to convert JPEG byte arrays into Image objects. This is static 
        // and only gets instantiated once.
        private static readonly ImageConverter _imageConverter = new ImageConverter();

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Image FromFile(string path)
        {
            var bytes = File.ReadAllBytes(path);
            var ms = new MemoryStream(bytes);
            var img = Image.FromStream(ms);
            return img;
        }

        public void AddToFindImages(MouseEventArgs e, string directoryPath)
        {
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                if (Directory.Exists(directoryPath))
                {
                    var fileNames = Directory.GetFiles(directoryPath, "*.bmp");
                    var findImage = $"{directoryPath}\\{fileNames.Length + 1}.bmp";
                    Logger.Info($" Adding FindImage {findImage}");
                    if (!File.Exists(findImage))
                    {
                        Logger.Info($" Adding FindImage {findImage}");
                        var temp = this.ScreenShot(e.X, e.Y, 8, 8, findImage);
                        var compare = this.ScreenShot(e.X - 100, e.Y - 100, 400, 300, "testing.bmp");
                        Logger.Info($" Captured find is found in Target? : {this.Find(compare, temp)}");
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public Point? Find(Bitmap haystack, Bitmap needle)
        {
            if (null == haystack || null == needle)
            {
                return null;
            }

            if (haystack.Width < needle.Width || haystack.Height < needle.Height)
            {
                return null;
            }

            var haystackArray = this.GetPixelArray(haystack);
            var needleArray = this.GetPixelArray(needle);

            foreach (var firstLineMatchPoint in this.FindMatch(haystackArray.Take(haystack.Height - needle.Height), needleArray[0]))
            {
                if (this.IsNeedlePresentAtLocation(haystackArray, needleArray, firstLineMatchPoint, 1))
                {
                    return firstLineMatchPoint;
                }
            }

            // Logger.Info($" {MethodBase.GetCurrentMethod().Name} Nothing Found");
            return null;
        }

        public bool FindBitmap(Bitmap searchBitmap, Bitmap withinBitmap, out Point point)
        {
            var innerTopLeft = withinBitmap.GetPixel(0, 0);

            for (var y = 0; y < searchBitmap.Height - withinBitmap.Height; y++)
            {
                for (var x = 0; x < searchBitmap.Width - withinBitmap.Width; x++)
                {
                    var clr = searchBitmap.GetPixel(x, y);
                    if (innerTopLeft == clr && this.IsInnerImage(searchBitmap, withinBitmap, x, y))
                    {
                        point = new Point(x, y); // Top left corner of the inner bitmap in searchBitmap - coordinates
                        return true;
                    }
                }
            }

            point = Point.Empty;
            return false;
        }

        public List<Bitmap> LoadAllBitmapsFromDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var allBitmaps = new List<Bitmap>();
            var fileNames = Directory.GetFiles(path, "*.bmp");
            var fileNamesList = new List<string>(fileNames);
            foreach (var fileName in fileNamesList)
            {
                // Bitmap fromFile = (Bitmap)_imageConverter.ConvertFrom(File.ReadAllBytes(fileName));
                var fromFile = (Bitmap)Image.FromFile(fileName);
                allBitmaps.Add(fromFile);
                Logger.Info($" loaded cat bitmap : {fileName} with index of {fileNamesList.IndexOf(fileName)}");
            }

            return allBitmaps;
        }

        public Dictionary<string, Bitmap> LoadAllBitmapsFromDirectoryAsDictionary(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            var allBitmaps = new Dictionary<string, Bitmap>();
            var fileNames = Directory.GetFiles(path, "*.bmp");
            var fileNamesList = new List<string>(fileNames);
            foreach (var fileName in fileNamesList)
            {
                allBitmaps.Add(fileName, (Bitmap)Image.FromFile(fileName));
                Logger.Info($" loaded bitmap : {fileName} with index of {fileNamesList.IndexOf(fileName)}");
            }

            return allBitmaps;
        }

        public void RenameBitmapsFromDirectory(string path)
        {
            var fileNames = Directory.GetFiles(path, "*.bmp");
            var counter = 0;
            try
            {
                foreach (var fileName in fileNames)
                {
                    Logger.Info($" {fileName} to {path}\\{counter}.bmp");
                    File.Move(fileName, $"{path}\\{counter}.bmp");
                    counter++;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message + e.InnerException?.Message);
            }
        }

        public void SaveAllBitmapsFromDirectory(string path)
        {
            var fileNames = Directory.GetFiles(path, "*.bmp");
            var counter = 0;
            foreach (var fileName in fileNames)
            {
                counter++;
                var bitmap = (Bitmap)Image.FromFile(fileName);
                bitmap.Save($"A{counter}.bmp", ImageFormat.Bmp);
            }
        }

        public Bitmap ScreenShot(string saveBmp, int xOffSet = 0, int yOffset = 0)
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            bounds.X = bounds.X - xOffSet;
            bounds.Y = bounds.Y - yOffset;
            var bmp = new Bitmap(
                bounds.Width,
                bounds.Height,
                PixelFormat.Format24bppRgb);
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.CopyFromScreen(
                    bounds.X,
                    bounds.Y,
                    0,
                    0,
                    bounds.Size,
                    CopyPixelOperation.SourceCopy);
            }

            bmp.Save(saveBmp);
            return bmp;
        }

        public Bitmap ScreenShot(int x, int y, int width, int height, string path)
        {
            var bmp = new Bitmap(
                width,
                height,
                PixelFormat.Format24bppRgb);
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.CopyFromScreen(
                    x,
                    y,
                    0,
                    0,
                    new Size(width, height),
                    CopyPixelOperation.SourceCopy);
            }

            bmp.Save(path, ImageFormat.Bmp);
            return bmp;
        }

        public void TakeScreenShot()
        {
            var bounds = Screen.PrimaryScreen.Bounds;
            using (var bmp = new Bitmap(
                bounds.Width,
                bounds.Height,
                PixelFormat.Format32bppArgb))
            using (var gfx = Graphics.FromImage(bmp))
            {
                gfx.CopyFromScreen(
                    bounds.X,
                    bounds.Y,
                    0,
                    0,
                    bounds.Size,
                    CopyPixelOperation.SourceCopy);
                bmp.Save("shot.png");
            }
        }

        private bool ContainSameElements(int[] first, int firstStart, int[] second, int secondStart, int length)
        {
            for (var i = 0; i < length; ++i)
            {
                if (first.Length <= i + firstStart || second.Length <= i + secondStart)
                {
                    return false;
                }

                if (first[i + firstStart] != second[i + secondStart])
                {
                    return false;
                }
            }

            return true;
        }

        private IEnumerable<Point> FindMatch(IEnumerable<int[]> haystackLines, int[] needleLine)
        {
            var y = 0;
            foreach (var haystackLine in haystackLines)
            {
                for (int x = 0, n = haystackLine.Length - needleLine.Length; x < n; ++x)
                {
                    if (this.ContainSameElements(haystackLine, x, needleLine, 0, needleLine.Length))
                    {
                        yield return new Point(x, y);
                    }
                }

                y += 1;
            }
        }

        private int[][] GetPixelArray(Bitmap bitmap)
        {
            var result = new int[bitmap.Height][];
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            for (var y = 0; y < bitmap.Height; ++y)
            {
                result[y] = new int[bitmap.Width];
                Marshal.Copy(bitmapData.Scan0 + y * bitmapData.Stride, result[y], 0, result[y].Length);
            }

            bitmap.UnlockBits(bitmapData);

            return result;
        }

        private bool IsInnerImage(Bitmap searchBitmap, Bitmap withinBitmap, int left, int top)
        {
            for (var y = top; y < top + withinBitmap.Height; y++)
            {
                for (var x = left; x < left + withinBitmap.Width; x++)
                {
                    if (searchBitmap.GetPixel(x, y) != withinBitmap.GetPixel(x - left, y - top)) return false;
                }
            }

            return true;
        }

        private bool IsNeedlePresentAtLocation(int[][] haystack, int[][] needle, Point point, int alreadyVerified)
        {
            // we already know that "alreadyVerified" lines already match, so skip them
            for (var y = alreadyVerified; y < needle.Length; ++y)
            {
                if (!this.ContainSameElements(haystack[y + point.Y], point.X, needle[y], 0, needle.Length))
                {
                    return false;
                }
            }

            return true;
        }
    }
}