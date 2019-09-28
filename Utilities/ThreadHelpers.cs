using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    public static class ThreadHelpers
    {
        private static Object lockObj = new Object();
        public static void ShowThreadInformation(String taskName)
        {
            String msg = null;
            Thread thread = Thread.CurrentThread;
            lock (lockObj)
            {
                msg = $"{taskName} thread information\n" +
                      $"   Background: {thread.IsBackground}\n" +
                      $"   Thread Pool: {thread.IsThreadPoolThread}\n" +
                      $"   Thread ID: {thread.ManagedThreadId}\n";
            }
            Console.WriteLine(msg);
        }

        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern IntPtr OpenThread(uint dwDesiredAccess, bool bInheritHandle, uint dwThreadId);


        [System.Runtime.InteropServices.DllImport("kernel32.dll")]
        static extern bool TerminateThread(IntPtr hThread, uint dwExitCode);


        private static Object lockDisplayMemory = new Object();
        public static void DisplayMemory(string context)
        {
            lock (lockDisplayMemory)
            {
                Debug.WriteLine($"----------------------------Memory Info @ {context} ----------------------------------------");
                Debug.WriteLine("Total memory: {0:###,###,###,##0} bytes", GC.GetTotalMemory(true));
                Debug.WriteLine("Private bytes {0}", System.Diagnostics.Process.GetCurrentProcess().PrivateMemorySize64);
                Debug.WriteLine("Handle count: {0}", System.Diagnostics.Process.GetCurrentProcess().HandleCount);
                Debug.WriteLine("Threads count: {0}", System.Diagnostics.Process.GetCurrentProcess().Threads.Count);
                Debug.WriteLine("------------------------------------------------------------------------------"); 
            }

        }
        private static Object lockDisplayThreadPoolInfo = new Object();

        public static void DisplayThreadPoolInfo(string context)
        {
            lock (lockDisplayThreadPoolInfo)
            {
                int workerThreads, completionPortThreads;
                ThreadPool.GetAvailableThreads(out workerThreads, out completionPortThreads);
                Debug.WriteLine($"----------------------------Thread Info @ {context} ----------------------------------------");
                Debug.WriteLine($"ThreadCount = {Process.GetCurrentProcess().Threads.Count},\n " +
                      $"Available worker threads = {workerThreads}, \n" +
                      $"Available completion port threads = {completionPortThreads}\n "); 
            }
        }

         private static Object lockTerminateThreadsAfterNMinutes = new Object();

        public static void TerminateThreadsAfterNMinutes(string context, int minutes)
        {
            lock (lockTerminateThreadsAfterNMinutes)
            {
                ProcessThreadCollection currentThreads = Process.GetCurrentProcess().Threads;
                Debug.WriteLine($"----------------------------TerminateThreadsAfterNMinutes  @ {context} ----------------------------------------");
                foreach (ProcessThread thread in currentThreads)
                {
                    Debug.WriteLine($"{thread.Id}, Runtime: {DateTime.Now.Subtract(thread.StartTime).Minutes} Min, {thread.ThreadState} ,{thread.WaitReason}, {thread.StartTime}   ");
                    if (thread.ThreadState == System.Diagnostics.ThreadState.Wait && thread.WaitReason == ThreadWaitReason.UserRequest &&
                        DateTime.Now.Subtract(thread.StartTime).Minutes > minutes)
                    {
                        IntPtr ptrThread = OpenThread(1, false, (uint)thread.Id);
#pragma warning disable CS0618 // Type or member is obsolete
                        if (AppDomain.GetCurrentThreadId() != thread.Id)
#pragma warning restore CS0618 // Type or member is obsolete
                        {
                            try
                            {
                                Debug.WriteLine($"Try Terminating {thread.Id}, Runtime: {DateTime.Now.Subtract(thread.StartTime).Minutes} Min, {thread.ThreadState} ,{thread.WaitReason}, {thread.StartTime}   ");

                                TerminateThread(ptrThread, 1);

                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(e.ToString());
                            }
                        }
                        else
                            Debug.WriteLine(". Not killing... It's the current thread!\n");
                    }

                } 
            }
        }
    }
}
