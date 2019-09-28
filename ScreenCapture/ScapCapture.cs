using System.Drawing;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Text;
using LZ4Sharp;
using AForge.Video.FFMPEG;
using System.IO;

namespace ScapLIB
{
    //Capture Class
    public class ScapCapture
    {
        //FPS timer
        internal Stopwatch Sw;
        //Compressor/Decompressor
        internal ILZ4Compressor Compressor;
        internal ILZ4Decompressor Decompressor;
        //Filename and Directory information
        internal string Imgext;
        internal string Vidext;
        internal string Dir;
        internal string Imgname;
        internal string Vidname;
        //Image and Video Information
        internal ScapVideoFormats Vidform;
        internal ScapImageFormats Imgform;
        //Capture coordinates and Size
        internal int X;
        internal int Y;
        internal int W;
        internal int H;
        //Number of allowed compression threads
        internal int NumThreads;
        //Runtime option
        internal int Rtime;
        //Number of captured frames
        internal int Frames;
        //Capture number
        internal int Capture;
        //Specified FPS
        internal int FPS;
        //Beginning frame number
        internal int FBegin;
        //Ending Frame number
        internal int FEnd;
        //Size of uncompressed image
        internal int Length;
        internal int Delay = 0;
        internal double AFPS;
        internal double DecompProgress;
        internal double EncodeProgress;
        internal List<Databank>[] MainDataLists;
        internal List<Databank> CompressedDataList;
        internal bool[] CompressorBools;
        //public control bools
        internal bool IsSoundEnabled = false;
        internal bool IsInitialized = false;
        internal bool IsDecompressed = false;
        internal bool IsEncoded = false;
        internal bool IsMultiThreaded = false;
        internal bool HasCaptured = false;
        //Init Methods
        public ScapCapture(bool RecordMic)
        {
            IsSoundEnabled = RecordMic;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            Dir = Directory.GetCurrentDirectory() + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = ScapVideoFormats.Default;
            Imgform = ScapImageFormats.Bmp;
            NumThreads = 1;
            X = 0;
            Y = 0;
            W = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            H = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = 50;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            MainDataLists = new List<Databank>[1];
            MainDataLists[0] = new List<Databank>();
            MainDataLists[0].Capacity = 25;
            CompressedDataList = new List<Databank>();
            CompressorBools = new bool[1];
            CompressorBools[0] = false;
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            Dir = Directory.GetCurrentDirectory() + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = ScapVideoFormats.Default;
            Imgform = ScapImageFormats.Bmp;
            NumThreads = NumberofThreads;
            X = 0;
            Y = 0;
            W = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            H = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = 50;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            Dir = Directory.GetCurrentDirectory() + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = ScapVideoFormats.Default;
            Imgform = ScapImageFormats.Bmp;
            NumThreads = NumberofThreads;
            X = 0;
            Y = 0;
            W = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            H = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond, ScapVideoFormats VideoFormat)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            Dir = Directory.GetCurrentDirectory() + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = VideoFormat;
            Imgform = ScapImageFormats.Bmp;
            NumThreads = NumberofThreads;
            X = 0;
            Y = 0;
            W = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            H = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond, ScapVideoFormats VideoFormat, ScapImageFormats ImageFormat)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            Dir = Directory.GetCurrentDirectory() + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = VideoFormat;
            Imgform = ImageFormat;
            NumThreads = NumberofThreads;
            X = 0;
            Y = 0;
            W = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            H = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond, ScapVideoFormats VideoFormat, ScapImageFormats ImageFormat, int Xcoord, int Ycoord, int CaptureWidth, int CaptureHeight)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            Dir = Directory.GetCurrentDirectory() + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = VideoFormat;
            Imgform = ImageFormat;
            NumThreads = NumberofThreads;
            X = Xcoord;
            Y = Ycoord;
            W = CaptureWidth;
            H = CaptureHeight;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond, ScapVideoFormats VideoFormat, ScapImageFormats ImageFormat, int Xcoord, int Ycoord, int CaptureWidth, int CaptureHeight, string CaptureDirectory)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = ".bmp";
            Vidext = ".avi";
            if (!Directory.Exists(CaptureDirectory))
            {
                Directory.CreateDirectory(CaptureDirectory);
                Dir = CaptureDirectory + "\\";
            }
            else Dir = CaptureDirectory + "\\";
            Imgname = "";
            Vidname = "Vid";
            Vidform = VideoFormat;
            Imgform = ImageFormat;
            NumThreads = 1;
            X = Xcoord;
            Y = Ycoord;
            W = CaptureWidth;
            H = CaptureHeight;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond, ScapVideoFormats VideoFormat, ScapImageFormats ImageFormat, int Xcoord, int Ycoord, int CaptureWidth, int CaptureHeight, string CaptureDirectory, string ImageFilename, string VideoFilename)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = "";
            Vidext = "";
            if (!Directory.Exists(CaptureDirectory))
            {
                Directory.CreateDirectory(CaptureDirectory);
                Dir = CaptureDirectory + "\\";
            }
            else Dir = CaptureDirectory + "\\";
            Imgname = ImageFilename;
            Vidname = VideoFilename;
            Vidform = VideoFormat;
            Imgform = ImageFormat;
            NumThreads = 1;
            X = Xcoord;
            Y = Ycoord;
            W = CaptureWidth;
            H = CaptureHeight;
            Rtime = -1;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        public ScapCapture(bool RecordMic, int NumberofThreads, int FramesPerSecond, ScapVideoFormats VideoFormat, ScapImageFormats ImageFormat, int Xcoord, int Ycoord, int CaptureWidth, int CaptureHeight, string CaptureDirectory, string ImageFilename, string VideoFilename, int Runtime)
        {
            IsSoundEnabled = RecordMic;
            IsMultiThreaded = true;
            Sw = new Stopwatch();
            Compressor = LZ4CompressorFactory.CreateNew();
            Decompressor = LZ4DecompressorFactory.CreateNew();
            Imgext = "";
            Vidext = "";
            if (!Directory.Exists(CaptureDirectory))
            {
                Directory.CreateDirectory(CaptureDirectory);
                Dir = CaptureDirectory + "\\";
            }
            else Dir = CaptureDirectory + "\\";
            Imgname = ImageFilename;
            Vidname = VideoFilename;
            Vidform = VideoFormat;
            Imgform = ImageFormat;
            NumThreads = 1;
            X = Xcoord;
            Y = Ycoord;
            W = CaptureWidth;
            H = CaptureHeight;
            Rtime = Runtime;
            Frames = 0;
            Capture = 0;
            FPS = FramesPerSecond;
            FBegin = 0;
            FEnd = -1;
            Length = 0;
            DecompProgress = 0;
            EncodeProgress = 0;
            CompressedDataList = new List<Databank>();
            MainDataLists = new List<Databank>[NumThreads];
            CompressorBools = new bool[NumThreads];
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c] = new List<Databank>();
                MainDataLists[c].Capacity = 25;
                CompressorBools[c] = false;
            }
            IsInitialized = true;
            IsDecompressed = false;
            IsEncoded = false;
            IsMultiThreaded = false;
            HasCaptured = false;
        }
        //Set Options Methods
        public void EnableMultipleDesktop()
        {
            throw new NotImplementedException();
        }
        //Release Resources Method
        public void Dispose()
        {
            Imgext = "";
            Vidext = "";
            Dir = "";
            Imgname = "";
            Vidname = "";
            for (int c = 0; c < NumThreads; c++)
            {
                MainDataLists[c].Clear();
            }
            MainDataLists = new List<Databank>[0];
            CompressorBools = new bool[0];
            CompressedDataList.Clear();
            Length = 0;
            IsDecompressed = false;
            IsEncoded = false;
            IsInitialized = false;
            IsMultiThreaded = false;
            Frames = 0;
            AFPS = 0;
            DecompProgress = 0;
        }
    }
    //Core Capture Class
    public static class ScapCore
    {
        public static bool StartCapture()
        {
            if (ScapBackendConfig.IsIntialized && !ScapBackendConfig.IsCapturing)
            {
                ScapBackendConfig.DefaultCapture.Frames = 0;
                ScapBackendConfig.DefaultCapture.HasCaptured = true;
                ScapBackendConfig.DefaultCapture.Sw.Reset();
                ScapBackendConfig.DefaultCapture.Sw.Start();
                ScapBackendConfig.IsCapturing = true;
                if (ScapBackendConfig.DefaultCapture.IsSoundEnabled)
                {
                    StartAudioRec();
                }
                return true;
            }
            else return false;
        }
        public static bool StopCapture()
        {
            if (ScapBackendConfig.IsCapturing)
            {
                ScapBackendConfig.IsCapturing = false;
                ScapBackendConfig.DefaultCapture.Sw.Stop();
                if (ScapBackendConfig.DefaultCapture.IsSoundEnabled)
                {
                    StopAudioRec();
                }
                return true;
            }
            return false;
        }


        public static bool PauseCapture()
        {
            if (ScapBackendConfig.IsCapturing)
            {
                ScapBackendConfig.IsPaused = true;
                ScapBackendConfig.DefaultCapture.Sw.Stop();
                if (ScapBackendConfig.DefaultCapture.IsSoundEnabled)
                {
                    PauseAudioRec();
                }
                return true;
            }
            return false;
        }
        public static bool ResumeCapture()
        {
            if (ScapBackendConfig.IsPaused)
            {
                ScapBackendConfig.IsPaused = false;
                ScapBackendConfig.DefaultCapture.Sw.Start();
                if (ScapBackendConfig.DefaultCapture.IsSoundEnabled)
                {
                    ResumeAudioRec();
                }
                return true;
            }
            return false;
        }

        public static void StartAudioRec()
        {
            Winmm.mciSendString("open new Type waveaudio Alias recsound", "", 0, 0);
            Winmm.mciSendString("record recsound", "", 0, 0);
        }
        public static void StopAudioRec()
        {
            string test = string.Format("save recsound \"{0}\"", ScapBackendConfig.DefaultCapture.Dir + "AudioCap.wav");
            Winmm.mciSendString(string.Format("{0}", test), "", 0, 0);
            Winmm.mciSendString("close recsound ", "", 0, 0);
        }

        public static void PauseAudioRec()
        {
            Winmm.mciSendString("pause recsound", "", 0, 0);
        }
        public static void ResumeAudioRec()
        {
            Winmm.mciSendString("record recsound", "", 0, 0);
        }

        public static bool DecompressCapture(bool SaveCompressedFiles)
        {
            if (ScapBackendConfig.DefaultCapture.HasCaptured && !ScapBackendConfig.IsCapturing)
            {
                ThreadPool.QueueUserWorkItem(ScapBackendConfig.DecompCallBack, SaveCompressedFiles);
                return true;
            }
            return false;
        }
        public static bool EncodeCapture(bool SaveImages)
        {
            if (ScapBackendConfig.DefaultCapture.HasCaptured && ScapBackendConfig.DefaultCapture.IsDecompressed)
            {
                ThreadPool.QueueUserWorkItem(ScapBackendConfig.EncodeCallBack, SaveImages);
                return true;
            }
            return false;
        }

        public static byte[] CaptureSingleFrameArray()
        {
            MemoryStream tempms = new MemoryStream();
            IntPtr handle = User.GetDesktopWindow();
            IntPtr hdcSrc = User.GetWindowDC(handle);
            IntPtr hdcDest = GDI.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI.CreateCompatibleBitmap(hdcSrc, ScapBackendConfig.DefaultCapture.W, ScapBackendConfig.DefaultCapture.H);
            IntPtr hOld = GDI.SelectObject(hdcDest, hBitmap);
            GDI.BitBlt(hdcDest, 0, 0, ScapBackendConfig.DefaultCapture.W, ScapBackendConfig.DefaultCapture.H, hdcSrc, ScapBackendConfig.DefaultCapture.X, ScapBackendConfig.DefaultCapture.Y, GDI.SRCCOPY | GDI.CAPTUREBLT);
            GDI.SelectObject(hdcDest, hOld);
            GDI.DeleteDC(hdcDest);
            User.ReleaseDC(handle, hdcSrc);
            //C# Image Handling
            System.Drawing.Image Img2 = System.Drawing.Image.FromHbitmap(hBitmap);
            Img2.Save(tempms, System.Drawing.Imaging.ImageFormat.Bmp);
            return tempms.GetBuffer();
        }
        public static MemoryStream CaptureSingleFrameStream()
        {
            MemoryStream tempms = new MemoryStream();
            IntPtr handle = User.GetDesktopWindow();
            IntPtr hdcSrc = User.GetWindowDC(handle);
            IntPtr hdcDest = GDI.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI.CreateCompatibleBitmap(hdcSrc, ScapBackendConfig.DefaultCapture.W, ScapBackendConfig.DefaultCapture.H);
            IntPtr hOld = GDI.SelectObject(hdcDest, hBitmap);
            GDI.BitBlt(hdcDest, 0, 0, ScapBackendConfig.DefaultCapture.W, ScapBackendConfig.DefaultCapture.H, hdcSrc, ScapBackendConfig.DefaultCapture.X, ScapBackendConfig.DefaultCapture.Y, GDI.SRCCOPY | GDI.CAPTUREBLT);
            GDI.SelectObject(hdcDest, hOld);
            GDI.DeleteDC(hdcDest);
            User.ReleaseDC(handle, hdcSrc);
            //C# Image Handling
            System.Drawing.Image Img2 = System.Drawing.Image.FromHbitmap(hBitmap);
            Img2.Save(tempms, System.Drawing.Imaging.ImageFormat.Bmp);
            return tempms;
        }
        public static System.Drawing.Bitmap CaptureSingleFrameBitmap()
        {
            IntPtr handle = User.GetDesktopWindow();
            IntPtr hdcSrc = User.GetWindowDC(handle);
            IntPtr hdcDest = GDI.CreateCompatibleDC(hdcSrc);
            IntPtr hBitmap = GDI.CreateCompatibleBitmap(hdcSrc, ScapBackendConfig.DefaultCapture.W, ScapBackendConfig.DefaultCapture.H);
            IntPtr hOld = GDI.SelectObject(hdcDest, hBitmap);
            GDI.BitBlt(hdcDest, 0, 0, ScapBackendConfig.DefaultCapture.W, ScapBackendConfig.DefaultCapture.H, hdcSrc, ScapBackendConfig.DefaultCapture.X, ScapBackendConfig.DefaultCapture.Y, GDI.SRCCOPY | GDI.CAPTUREBLT);
            GDI.SelectObject(hdcDest, hOld);
            GDI.DeleteDC(hdcDest);
            User.ReleaseDC(handle, hdcSrc);
            //C# Image Handling
            System.Drawing.Bitmap Img2 = System.Drawing.Bitmap.FromHbitmap(hBitmap);
            return Img2;
        }

        public static byte[] DecompressSingleFrame(byte[] Frame, int LengthDecompressed)
        {
            byte[] temp = new byte[LengthDecompressed];
            ScapBackendConfig.DefaultCapture.Decompressor.DecompressKnownSize(Frame, temp, LengthDecompressed);
            return temp;
        }
        public static byte[] CompressSingleFrame(byte[] Frame)
        {
            byte[] temp;
            temp = ScapBackendConfig.DefaultCapture.Compressor.Compress(Frame);
            return temp;
        }

        public static double GetDecompressionProgress()
        {
            return ScapBackendConfig.DefaultCapture.DecompProgress;
        }
        public static double GetEncodeProgress()
        {
            return ScapBackendConfig.DefaultCapture.EncodeProgress;
        }
        public static int GetCaptureCount()
        {
            return ScapBackendConfig.DefaultCapture.Frames;
        }
        public static double GetActualFPS()
        {
            if (ScapBackendConfig.DefaultCapture.Sw.ElapsedMilliseconds != 0)
            {
                ScapBackendConfig.DefaultCapture.AFPS = (ScapBackendConfig.DefaultCapture.Frames * 1000) / ScapBackendConfig.DefaultCapture.Sw.ElapsedMilliseconds;
                return ScapBackendConfig.DefaultCapture.AFPS;
            }
            else return -1;
        }
    }
    //Multi Threaded Functions Class
    public static class ScapBackendConfig
    {
        //Capture Data
        internal static ScapCapture DefaultCapture;
        //Control timer
        internal static System.Windows.Forms.Timer ThreadTimer;
        //GDI Image Data
        private static IntPtr handle;
        private static IntPtr hdcSrc;
        private static IntPtr hdcDest;
        private static IntPtr hBitmap;
        private static IntPtr hOld;
        //Managed Image data
        private static MemoryStream ms;
        private static System.Drawing.Image Img;
        private static FileIO IO;
        private static VideoCodec VidCodec;
        private static System.Drawing.Imaging.ImageFormat ImageCodec;
        //Bools
        internal static bool Writing = false;
        internal static bool Compressing = false;
        internal static bool Decompressing = false;
        internal static bool Capturing = false;
        internal static bool Encoding = false;
        internal static bool IsSoundEnabled = false;
        internal static bool IsIntialized = false;
        internal static bool IsCapturing = false;
        internal static bool IsPaused = false;
        //Thread Pool Callbacks
        internal static readonly WaitCallback CaptureCallBack = new WaitCallback(Capture);
        internal static readonly WaitCallback CompressCallBack = new WaitCallback(Compress);
        internal static readonly WaitCallback WriteCallBack = new WaitCallback(Write);
        internal static readonly WaitCallback DecompCallBack = new WaitCallback(Decompress);
        internal static readonly WaitCallback EncodeCallBack = new WaitCallback(Encode);
        //init methods 
        public static void ScapBackendSetup(ScapCapture Capture)
        {
            DefaultCapture = Capture;
            IsSoundEnabled = DefaultCapture.IsSoundEnabled;
            ms = new MemoryStream();
            SetVideoCodec();
            SetImageCodec();
            ThreadTimer = new System.Windows.Forms.Timer();
            ThreadTimer.Interval = 1000 / DefaultCapture.FPS;
            ThreadTimer.Tick += new EventHandler(ThreadTimer_Tick);
            ThreadTimer.Enabled = true;
            IsIntialized = true;
        }
        public static void Dispose()
        {
            DefaultCapture.Dispose();
            ThreadTimer.Enabled = false;
            ms.Dispose();
            Img.Dispose();
            IO.Dispose();
            IsIntialized = false;
        }
        //misc init helpers
        private static void SetVideoCodec()
        {
            if (DefaultCapture.Vidform == ScapVideoFormats.Default)
            {
                VidCodec = VideoCodec.Default;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.FLV1)
            {
                VidCodec = VideoCodec.FLV1;
                DefaultCapture.Vidext = ".flv";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.H263P)
            {
                VidCodec = VideoCodec.H263P;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.MPEG2)
            {
                VidCodec = VideoCodec.MPEG2;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.MPEG4)
            {
                VidCodec = VideoCodec.MPEG4;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.MSMPEG4v2)
            {
                VidCodec = VideoCodec.MSMPEG4v2;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.MSMPEG4v3)
            {
                VidCodec = VideoCodec.MSMPEG4v3;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.RAW)
            {
                VidCodec = VideoCodec.Raw;
                DefaultCapture.Vidext = ".avi";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.WMV1)
            {
                VidCodec = VideoCodec.WMV1;
                DefaultCapture.Vidext = ".wmv";
            }
            if (DefaultCapture.Vidform == ScapVideoFormats.WMV2)
            {
                VidCodec = VideoCodec.WMV2;
                DefaultCapture.Vidext = ".wmv";
            }

        }
        private static void SetImageCodec()
        {
            if (DefaultCapture.Imgform == ScapImageFormats.Bmp)
            {
                ImageCodec = System.Drawing.Imaging.ImageFormat.Bmp;
                DefaultCapture.Imgext = ".bmp";
            }
            if (DefaultCapture.Imgform == ScapImageFormats.Gif)
            {
                ImageCodec = System.Drawing.Imaging.ImageFormat.Gif;
                DefaultCapture.Imgext = ".gif";
            }
            if (DefaultCapture.Imgform == ScapImageFormats.Jpeg)
            {
                ImageCodec = System.Drawing.Imaging.ImageFormat.Jpeg;
                DefaultCapture.Imgext = ".jpg";
            }
            if (DefaultCapture.Imgform == ScapImageFormats.PNG)
            {
                ImageCodec = System.Drawing.Imaging.ImageFormat.Png;
                DefaultCapture.Imgext = ".png";
            }
            if (DefaultCapture.Imgform == ScapImageFormats.Tiff)
            {
                ImageCodec = System.Drawing.Imaging.ImageFormat.Tiff;
                DefaultCapture.Imgext = ".Tiff";
            }
        }
        //Control Timer Callback
        private static void ThreadTimer_Tick(object sender, EventArgs e)
        {
            if (DefaultCapture.Rtime != -1 && DefaultCapture.Sw.ElapsedMilliseconds >= DefaultCapture.Rtime)
            {
                IsCapturing = false;
            }
            if (IsCapturing && !IsPaused && !Capturing)
            {
                Capturing = true;
                ThreadPool.QueueUserWorkItem(CaptureCallBack, null);
            }
            for (int c = 0; c < DefaultCapture.NumThreads; c++)
            {
                if (DefaultCapture.MainDataLists[c].Count != 0 && !DefaultCapture.CompressorBools[c])
                {
                    DefaultCapture.CompressorBools[c] = true;
                    ThreadPool.QueueUserWorkItem(CompressCallBack, (object)c);
                }
            }
            if (DefaultCapture.CompressedDataList.Count != 0 && !Writing)
            {
                Writing = true;
                ThreadPool.QueueUserWorkItem(WriteCallBack, null);
            }
        }
        //Thread Callbacks
        private static void Capture(object s)
        {
            Capturing = true;
            if (ms.Position != 0)
            {
                ms.Seek(0, SeekOrigin.Begin);
            }
            handle = User.GetDesktopWindow();
            hdcSrc = User.GetWindowDC(handle);
            hdcDest = GDI.CreateCompatibleDC(hdcSrc);
            hBitmap = GDI.CreateCompatibleBitmap(hdcSrc, DefaultCapture.W, DefaultCapture.H);
            hOld = GDI.SelectObject(hdcDest, hBitmap);
            GDI.BitBlt(hdcDest, 0, 0, DefaultCapture.W, DefaultCapture.H, hdcSrc, DefaultCapture.X, DefaultCapture.Y, GDI.SRCCOPY | GDI.CAPTUREBLT);
            GDI.SelectObject(hdcDest, hOld);
            GDI.DeleteDC(hdcDest);
            User.ReleaseDC(handle, hdcSrc);
            Img = System.Drawing.Image.FromHbitmap(hBitmap);
            Img.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            //Get Length of Uncompressed Data for Decompression
            if (DefaultCapture.Length == 0 || DefaultCapture.Length != (int)ms.Length)
            {
                DefaultCapture.Length = (int)ms.Length;
            }
            for (int c = 0; c < DefaultCapture.NumThreads; c++)
            {
                if (DefaultCapture.MainDataLists[c].Count <= 20)
                {
                    DefaultCapture.MainDataLists[c].Add(new Databank(DefaultCapture.Frames, ms.GetBuffer()));
                    break;
                }
            }
            Img.Dispose();
            GDI.DeleteObject(hBitmap);
            //Increment Capture Number and Set Capturing bool to false
            DefaultCapture.Frames++;
            Capturing = false;
        }
        private static void Compress(object s)
        {
            DefaultCapture.CompressorBools[(int)s] = true;
            DefaultCapture.CompressedDataList.Add(new Databank(DefaultCapture.MainDataLists[(int)s][0].CapNumber, DefaultCapture.Compressor.Compress(DefaultCapture.MainDataLists[(int)s][0].Buff)));
            DefaultCapture.MainDataLists[(int)s].RemoveAt(0);
            DefaultCapture.CompressorBools[(int)s] = false;
        }
        private static void Write(object s)
        {
            Writing = true;
            IO = new FileIO(DefaultCapture.CompressedDataList[0].Buff);
            IO.Write(DefaultCapture.CompressedDataList[0].Buff.Length, DefaultCapture.Dir + DefaultCapture.Capture + "-" + DefaultCapture.CompressedDataList[0].CapNumber);
            DefaultCapture.CompressedDataList.RemoveAt(0);
            IO.Dispose();
            Writing = false;
        }
        private static void Decompress(object s)
        {
            for (int c = 0; c < DefaultCapture.Frames; c++)
            {
                DefaultCapture.DecompProgress = (double)c / ((double)DefaultCapture.Frames - 1);
                byte[] tempbuff = File.ReadAllBytes(DefaultCapture.Dir + DefaultCapture.Capture + "-" + c);
                byte[] decompbuff = new byte[DefaultCapture.Length];
                DefaultCapture.Decompressor.DecompressKnownSize(tempbuff, decompbuff, DefaultCapture.Length);
                MemoryStream i = new MemoryStream(decompbuff);
                MemoryStream j = new MemoryStream();
                Img = System.Drawing.Bitmap.FromStream(i);
                Img.Save(j, ImageCodec);
                decompbuff = new byte[j.Length];
                decompbuff = j.GetBuffer();
                IO = new FileIO(decompbuff);
                IO.Write(decompbuff.Length, DefaultCapture.Dir + DefaultCapture.Capture + "-" + DefaultCapture.Imgname + c + DefaultCapture.Imgext);
                IO.Dispose();
                i.Dispose();
                Img.Dispose();
            }
            if (!(bool)s)
            {
                for (int c = 0; c < DefaultCapture.Frames; c++)
                {
                    File.Delete(DefaultCapture.Dir + DefaultCapture.Capture + "-" + c);
                }
            }
            DefaultCapture.IsDecompressed = true;
        }
        private static void Encode(object s)
        {
            VideoFileWriter writer = new VideoFileWriter();

            writer.Open(DefaultCapture.Dir + DefaultCapture.Capture + "-" + DefaultCapture.Vidname + DefaultCapture.Vidext, DefaultCapture.W, DefaultCapture.H, (int)ScapCore.GetActualFPS(), VidCodec);
            System.Drawing.Bitmap bmp;
            for (int c = 0; c < DefaultCapture.Frames; c++)
            {
                DefaultCapture.EncodeProgress = (double)c / ((double)DefaultCapture.Frames - 1);
                bmp = (System.Drawing.Bitmap)System.Drawing.Bitmap.FromFile(DefaultCapture.Dir + DefaultCapture.Capture + "-" + DefaultCapture.Imgname + c + DefaultCapture.Imgext);
                writer.WriteVideoFrame(bmp);
                bmp.Dispose();
            }
            writer.Close();
            if (!(bool)s)
            {
                for (int c = 0; c < DefaultCapture.Frames; c++)
                {
                    File.Delete(DefaultCapture.Dir + DefaultCapture.Capture + "-" + DefaultCapture.Imgname + c + DefaultCapture.Imgext);
                }
            }
        }
    }
}

