namespace KeyboardMouse
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;

    public static class InterceptMouse

    {
        private const int WH_MOUSE_LL = 14;

        private static LowLevelMouseProc _proc = HookCallback;

        private static IntPtr _hookID = IntPtr.Zero;

        public static event MouseEventHandler OnMouseLeftButtonDown = delegate { };
        public static void Start()
        {
            _hookID = SetHook(_proc);
        }

        public static void Stop()
        {
            UnhookWindowsHookEx(_hookID);
        }
       
        private static IntPtr SetHook(LowLevelMouseProc proc)
        {
            using (var curProcess = Process.GetCurrentProcess())
            {
                using (var curModule = curProcess.MainModule)
                {
                    return SetWindowsHookEx(
                        WH_MOUSE_LL,
                        proc,
                        GetModuleHandle(curModule.ModuleName),
                        0);
                }
            }
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam,IntPtr lParam)
        {
            if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
            {
                var hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
                Console.WriteLine(hookStruct.pt.x + ", " + hookStruct.pt.y);
                OnMouseLeftButtonDown(null, new MouseEventArgs(MouseButtons.Left, 1, hookStruct.pt.x, hookStruct.pt.y,0));
            }

            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public static void FolllowMouseWindow()
        {
            //IntPtr hwndTip = NativeMethods.CreateWindowEx(null, TOOLTIPS_CLASS, NULL,
            //           WS_POPUP | TTS_NOPREFIX | TTS_ALWAYSTIP,
            //           CW_USEDEFAULT, CW_USEDEFAULT,
            //           CW_USEDEFAULT, CW_USEDEFAULT,
            //           hwndParent, NULL, hinstMyDll,
            //           NULL);

            //SetWindowPos(hwndTip, HWND_TOPMOST, 0, 0, 0, 0,
            //         SWP_NOMOVE | SWP_NOSIZE | SWP_NOACTIVATE);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(
            int idHook,
            LowLevelMouseProc lpfn,
            IntPtr hMod,
            uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(
            IntPtr hhk,
            int nCode,
            IntPtr wParam,
            IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        private enum MouseMessages

        {
            WM_LBUTTONDOWN = 0x0201,

            WM_LBUTTONUP = 0x0202,

            WM_MOUSEMOVE = 0x0200,

            WM_MOUSEWHEEL = 0x020A,

            WM_RBUTTONDOWN = 0x0204,

            WM_RBUTTONUP = 0x0205
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT

        {
            public readonly int x;

            public readonly int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT

        {
            public POINT pt;

            public readonly uint mouseData;

            public readonly uint flags;

            public readonly uint time;

            public readonly IntPtr dwExtraInfo;
        }
    }
}