using System;
using System.Runtime.InteropServices;
namespace ScapLIB
{
    using Microsoft.Win32.SafeHandles;

    internal class GDI
    {
        //Constants
        public const int SRCCOPY = 0x00CC0020;
        public const int CAPTUREBLT = 0x40000000;
        //DLL Import
        [DllImport("gdi32.dll")]
        public static extern bool BitBlt(IntPtr hObject, int nXDest, int nYDest, int nWidth, int nHeight, IntPtr hObjectSource, int nXSrc, int nYSrc, int dwRop);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr hDC, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        public static extern IntPtr CreateCompatibleDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteDC(IntPtr hDC);
        [DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);
        [DllImport("gdi32.dll")]
        public static extern IntPtr SelectObject(IntPtr hDC, IntPtr hObject);
    }
    //User32 InterOp Class
    internal class User
    {
        //DLL Import
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr ReleaseDC(IntPtr hWnd, IntPtr hDC);
        [DllImport("user32.dll")]
        public static extern int GetAsyncKeyState(int vKey);
    }
    //Kernel32 InterOp Class
    internal class Kernel
    {
        //DLL Import
        [DllImport("kernel32", SetLastError = true)]
        public static extern SafeFileHandle CreateFile(string FileName, uint DesiredAccess, uint ShareMode, uint SecurityAttributes, uint CreationDisposition, uint FlagsAndAttributes, int hTemplateFile);
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool WriteFile(SafeFileHandle hFile,Byte[] aBuffer,UInt32 cbToWrite,ref UInt32 cbThatWereWritten,IntPtr pOverlapped);[DllImport("kernel32", SetLastError = true)]
        internal static extern Int32 CloseHandle(SafeFileHandle hObject);
    }
    ////Winmm InterOp Class
    internal class Winmm
    {
        [DllImport("winmm.dll", EntryPoint = "mciSendStringA", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true)]
        internal static extern int mciSendString(string lpstrCommand, string lpstrReturnString, int uReturnLength, int hwndCallback);
    }
}
