namespace KeyboardMouse
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;

    public static class ApplicationInput
    {
        public static bool SetFocusToExternalApp(string strProcessName)
        {
            var arrProcesses = Process.GetProcessesByName(strProcessName);
            if (arrProcesses.Length > 0)
            {
                var ipHwnd = arrProcesses[0].MainWindowHandle;
                Thread.Sleep(100);
                NativeMethods.SetForegroundWindow(ipHwnd);
                return true;
            }
            return false;
        }


        public static string GetActiveProcessName()
        {
            Process process = null;
            try
            {
                IntPtr hwnd = NativeMethods.GetForegroundWindow();
                uint pid;
                NativeMethods.GetWindowThreadProcessId(hwnd, out pid);
                process = Process.GetProcessById((int)pid);
                return process.MainModule.FileName;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            return string.Empty;
        }

        public static bool FlashWindow(string processName, int count = 10, int rate = 1)
        {
            var arrProcesses = Process.GetProcessesByName(processName);
            if (arrProcesses.Length > 0)
            {
                var ipHwnd = arrProcesses[0].MainWindowHandle;
                return FlashWindow(ipHwnd, NativeMethods.FlashWindowFlags.FLASHW_ALL, (uint)count, (uint)rate);
            }
            return false;
        }

        public static bool StopFlashingWindow(string processName)
        {
            var arrProcesses = Process.GetProcessesByName(processName);
            if (arrProcesses.Length > 0)
            {
                var ipHwnd = arrProcesses[0].MainWindowHandle;
                return StopFlashingWindow(ipHwnd);
            }
            return false;
        }

        private static bool FlashWindow(
            IntPtr hWnd,
            NativeMethods.FlashWindowFlags fOptions,
            uint FlashCount,
            uint FlashRate)
        {
            if (IntPtr.Zero != hWnd)
            {
                var fi = new NativeMethods.FLASHWINFO();
                fi.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.FLASHWINFO));
                fi.dwFlags = fOptions;
                fi.uCount = FlashCount;
                fi.dwTimeout = FlashRate;
                fi.hwnd = hWnd;

                return NativeMethods.FlashWindowEx(ref fi);
            }
            return false;
        }

        private static bool StopFlashingWindow(IntPtr hWnd)
        {
            if (IntPtr.Zero != hWnd)
            {
                var fi = new NativeMethods.FLASHWINFO();
                fi.cbSize = (uint)Marshal.SizeOf(typeof(NativeMethods.FLASHWINFO));
                fi.dwFlags = (uint)NativeMethods.FlashWindowFlags.FLASHW_STOP;
                fi.hwnd = hWnd;

                return NativeMethods.FlashWindowEx(ref fi);
            }
            return false;
        }
    }
}