using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyboardMouse;

namespace WakeMeUpFromSleep
{
    using System.Threading;

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Turning off monitor in 5 seconds...");
            Thread.Sleep(5000);
            // NativeMethods.SetSuspendState(false, true, false); // This will log off user

            MonitorControl.SetMonitorInState(MonitorControl.MonitorState.MonitorStateStandBy);

            
            Console.WriteLine("Waiting for computer to wake up for a minute");
            WakeUpTimer.SetWaitForWakeUpTime(1);
            Console.WriteLine("Wake Up the computer- turn the monitor on");
            MonitorControl.WakeUp();
            Console.WriteLine("computer awake - monitor was turned on successfully if you can read this");
            Console.ReadLine();
        }
    }
}
