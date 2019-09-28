using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardMouse
{
   

    /// <summary>
    /// Helps to find the idle time, (in ticks) spent since the last user input
    /// </summary>
    public static class IdleTimeFinder
    {
        public static uint GetIdleTime()
        {
            NativeMethods.LASTINPUTINFO lastInPut = new NativeMethods.LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            NativeMethods.GetLastInputInfo(ref lastInPut);

            return ((uint)Environment.TickCount - lastInPut.dwTime);
        }
        /// <summary>
        /// Get the Last input time in ticks
        /// </summary>
        /// <returns></returns>
        public static long GetLastInputTimeInTicks()
        {
            NativeMethods.LASTINPUTINFO lastInPut = new NativeMethods.LASTINPUTINFO();
            lastInPut.cbSize = (uint)System.Runtime.InteropServices.Marshal.SizeOf(lastInPut);
            if (!NativeMethods.GetLastInputInfo(ref lastInPut))
            {
                throw new Exception(NativeMethods.GetLastError().ToString());
            }
            return lastInPut.dwTime;
        }

        public static TimeSpan GetLastInputTime()
        {
            return TimeSpan.FromTicks(GetLastInputTimeInTicks());
        }
    }
}
