namespace KeyboardMouse
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Windows.Forms;

    using Microsoft.Win32.SafeHandles;

    public static class NativeMethods
    {

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, uint dwFlags,int dwExtraInfo);

        [DllImport("user32.dll")]
        public static extern void mouse_event(Int32 dwFlags, Int32 dx, Int32 dy, Int32 dwData, UIntPtr dwExtraInfo);


        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point p);
        [DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref Point lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint ProcessId);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FlashWindowEx(ref FLASHWINFO pwfi);

        public struct LASTINPUTINFO
        {
            public uint cbSize;

            public uint dwTime;
        }

        [DllImport("User32.dll")]
        public static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

        [DllImport("Kernel32.dll")]
        public static extern uint GetLastError();

        [StructLayout(LayoutKind.Sequential)]
        public struct FLASHWINFO
        {
            /// <summary>
            /// The size of the structure in bytes.
            /// </summary>
            public uint cbSize;
            /// <summary>
            /// A Handle to the Window to be Flashed. The window can be either opened or minimized.
            /// </summary>
            public IntPtr hwnd;
            /// <summary>
            /// The Flash Status.
            /// </summary>
            public FlashWindowFlags dwFlags; //uint
                                             /// <summary>
                                             /// The number of times to Flash the window.
                                             /// </summary>
            public uint uCount;
            /// <summary>
            /// The rate at which the Window is to be flashed, in milliseconds. If Zero, the function uses the default cursor blink rate.
            /// </summary>
            public uint dwTimeout;
        }


        public enum FlashWindowFlags : uint
        {
            /// <summary>
            /// Stop flashing. The system restores the window to its original state.
            /// </summary>
            FLASHW_STOP = 0,

            /// <summary>
            /// Flash the window caption.
            /// </summary>
            FLASHW_CAPTION = 1,

            /// <summary>
            /// Flash the taskbar button.
            /// </summary>
            FLASHW_TRAY = 2,

            /// <summary>
            /// Flash both the window caption and taskbar button.
            /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags.
            /// </summary>
            FLASHW_ALL = 3,

            /// <summary>
            /// Flash continuously, until the FLASHW_STOP flag is set.
            /// </summary>
            FLASHW_TIMER = 4,

            /// <summary>
            /// Flash continuously until the window comes to the foreground.
            /// </summary>
            FLASHW_TIMERNOFG = 12
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern Int32 SendInput(Int32 cInputs, ref INPUT pInputs, Int32 cbSize);

        [StructLayout(LayoutKind.Explicit, Pack = 1, Size = 28)]
        internal struct INPUT
        {
            [FieldOffset(0)]
            public InputType dwType;

            [FieldOffset(4)]
            public MOUSEINPUT mi;

            [FieldOffset(4)]
            public KEYBDINPUT ki;

            [FieldOffset(4)]
            public HARDWAREINPUT hi;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct MOUSEINPUT
        {
            public Int32 dx;

            public Int32 dy;

            public Int32 mouseData;

            public MOUSEEVENTF dwFlags;

            public Int32 time;

            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct KEYBDINPUT
        {
            public Int16 wVk;

            public Int16 wScan;

            public KEYEVENTF dwFlags;

            public Int32 time;

            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct HARDWAREINPUT
        {
            public Int32 uMsg;

            public Int16 wParamL;

            public Int16 wParamH;
        }

        internal enum InputType : int
        {
            Mouse = 0,

            Keyboard = 1,

            Hardware = 2
        }

        [Flags()]
        internal enum MOUSEEVENTF : int
        {
            MOVE = 0x1,

            LEFTDOWN = 0x2,

            LEFTUP = 0x4,

            RIGHTDOWN = 0x8,

            RIGHTUP = 0x10,

            MIDDLEDOWN = 0x20,

            MIDDLEUP = 0x40,

            XDOWN = 0x80,

            XUP = 0x100,

            VIRTUALDESK = 0x400,

            WHEEL = 0x800,

            ABSOLUTE = 0x8000
        }

        [Flags()]
        public enum KEYEVENTF : int
        {
            EXTENDEDKEY = 1,

            KEYUP = 2,

            UNICODE = 4,

            SCANCODE = 8
        }

        /// <summary>The MapVirtualKey function translates (maps) a virtual-key code into a scan
        /// code or character value, or translates a scan code into a virtual-key code
        /// </summary>
        /// <param name="uCode">[in] Specifies the virtual-key code or scan code for a key.
        /// How this value is interpreted depends on the value of the uMapType parameter</param>
        /// <param name="uMapType">[in] Specifies the translation to perform. The value of this
        /// parameter depends on the value of the uCode parameter.</param>
        /// <returns>Either a scan code, a virtual-key code, or a character value, depending on
        /// the value of uCode and uMapType. If there is no translation, the return value is zero</returns>
        /// <remarks></remarks>
        [DllImport("User32.dll", SetLastError = false, CallingConvention = CallingConvention.StdCall,
            CharSet = CharSet.Auto)]
        public static extern UInt32 MapVirtualKey(UInt32 uCode, MapVirtualKeyMapTypes uMapType);


        /// <summary>The set of valid MapTypes used in MapVirtualKey
        /// </summary>
        /// <remarks></remarks>
        public enum MapVirtualKeyMapTypes : uint
        {
            /// <summary>uCode is a virtual-key code and is translated into a scan code.
            /// If it is a virtual-key code that does not distinguish between left- and
            /// right-hand keys, the left-hand scan code is returned.
            /// If there is no translation, the function returns 0.
            /// </summary>
            /// <remarks></remarks>
            MAPVK_VK_TO_VSC = 0x0,

            /// <summary>uCode is a scan code and is translated into a virtual-key code that
            /// does not distinguish between left- and right-hand keys. If there is no
            /// translation, the function returns 0.
            /// </summary>
            /// <remarks></remarks>
            MAPVK_VSC_TO_VK = 0x1,

            /// <summary>uCode is a virtual-key code and is translated into an unshifted
            /// character value in the low-order word of the return value. Dead keys (diacritics)
            /// are indicated by setting the top bit of the return value. If there is no
            /// translation, the function returns 0.
            /// </summary>
            /// <remarks></remarks>
            MAPVK_VK_TO_CHAR = 0x2,

            /// <summary>Windows NT/2000/XP: uCode is a scan code and is translated into a
            /// virtual-key code that distinguishes between left- and right-hand keys. If
            /// there is no translation, the function returns 0.
            /// </summary>
            /// <remarks></remarks>
            MAPVK_VSC_TO_VK_EX = 0x3,

            /// <summary>Not currently documented
            /// </summary>
            /// <remarks></remarks>
            MAPVK_VK_TO_VSC_EX = 0x4
        }

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CreateWindowEx(UInt32 dwExStyle , string lpClassName,string lpWindowName,
                        UInt32  dwStyle, int x,int y,int nWidth,int nHeight,IntPtr hWndParent,
                        IntPtr hMenu,IntPtr hInstance,IntPtr lpParam);

        public delegate void TimerCompleteDelegate();

        [DllImport("kernel32.dll")]
        public static extern SafeWaitHandle CreateWaitableTimer(IntPtr lpTimerAttributes, bool bManualReset, string lpTimerName);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWaitableTimer(SafeWaitHandle hTimer, [In] ref long pDueTime, int lPeriod, IntPtr pfnCompletionRoutine, IntPtr lpArgToCompletionRoutine, bool fResume);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool CancelWaitableTimer(IntPtr hTimer);

        [DllImport("user32")]
        public static extern void LockWorkStation();

        [DllImport("PowrProf.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern bool SetSuspendState(bool hiberate, bool forceCritical, bool disableWakeEvent);

        [DllImport("user32")]
        public static extern bool ExitWindowsEx(uint uFlags, uint dwReason);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd, int hMsg, int wParam, int lParam);

        


    }
}
