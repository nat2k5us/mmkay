using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardMouse
{
    using System.Threading;

    public static class MonitorControl
    {
        private const int MOUSEEVENTF_MOVE = 0x0001;
        public enum MonitorState
        {
            MonitorStateOn = -1,
            MonitorStateOff = 2,
            MonitorStateStandBy = 1
        }

        public static void SetMonitorInState(MonitorState state)
        {
            NativeMethods.SendMessage(0xFFFF, 0x112, 0xF170, (int)state);
            bool turnOff = true;   //set true if you want to turn off, false if on
            int WM_SYSCOMMAND = 0x112;
            int SC_MONITORPOWER = 0xF170;
            NativeMethods.SendMessage(0xFFFF, WM_SYSCOMMAND, SC_MONITORPOWER, (turnOff ? 2 : -1));
        }

        public static void WakeUp()
        {
            NativeMethods.mouse_event(MOUSEEVENTF_MOVE, 0, 1, 0, UIntPtr.Zero);
            Thread.Sleep(40);
            NativeMethods.mouse_event(MOUSEEVENTF_MOVE, 0, -1, 0, UIntPtr.Zero);
        }
    }
}
