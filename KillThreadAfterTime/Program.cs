using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KillThreadAfterTime
{
    class Program
    {
        // killing the thread after [ThreadTimeout], you can put 5min or any value
        static TimeSpan ThreadTimeout = new TimeSpan(0, 0, 10);
        // oThread start date
        static DateTime ThreadStartDate;
        // thread killer after the timeout
        static Thread ThreadTimeoutKiller = new Thread(new ParameterizedThreadStart(KillThreadAfterTimeout));

        static void DoWork()
        {
            try
            {
                ThreadStartDate = DateTime.Now;

                for (;;)    
                {
                    Console.WriteLine("I am working !");
                    Thread.Sleep(500);
                }
            }
            catch (ThreadAbortException)
            {
                Console.WriteLine("I am aborted !");
            }

        }

        static void KillThreadAfterTimeout(object oThread)
        {
            // Kill the thread after ThreadTimeout TimeSpan
            for (;;)
            {
                if (DateTime.Now > ThreadStartDate.AddTicks(ThreadTimeout.Ticks) && ((Thread)oThread).IsAlive)
                {
                    ((Thread)oThread).Abort();
                }

                if (!((Thread)oThread).IsAlive)
                {
                    break;
                }

                Thread.Sleep(500);

            }
        }


        static void Main(string[] args)
        {
            Thread oThread = new Thread(DoWork);
            oThread.Start();
            ThreadTimeoutKiller.Start(oThread);

            Console.ReadLine();
        }

    }

 
}
