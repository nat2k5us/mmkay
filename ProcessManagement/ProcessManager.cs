using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessManagement
{
    using System.Diagnostics;

    public static class ProcessManager
    {
        public static IntPtr GetProcessHandle(string processName)
        {
            IntPtr windowHandle = IntPtr.Zero;
            Process[] processes = Process.GetProcessesByName(processName);

            foreach (Process p in processes)
            {
                windowHandle = p.MainWindowHandle;
                break;
                // do something with windowHandle
            }

            return windowHandle;
        }

      
    }
}
