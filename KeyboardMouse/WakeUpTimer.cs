using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace KeyboardMouse
{
    public static class WakeUpTimer
    {

        public static void SetWaitForWakeUpTime(int minutes)
        {
            DateTime utc = DateTime.Now.AddMinutes(minutes);
            long duetime = utc.ToFileTime();

            using (var handle = NativeMethods.CreateWaitableTimer(IntPtr.Zero, true, "wakeuptimer"))
            {
                if (NativeMethods.SetWaitableTimer(handle, ref duetime, 0, IntPtr.Zero, IntPtr.Zero, true))
                {
                    using (EventWaitHandle wh = new EventWaitHandle(false, EventResetMode.AutoReset))
                    {
                        wh.SafeWaitHandle = handle;
                        wh.WaitOne();
                    }
                }
                else
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }
            }

            // You could make it a recursive call here, setting it to 1 hours time or similar
            Console.WriteLine("Wake up call");
        }
    }
}

