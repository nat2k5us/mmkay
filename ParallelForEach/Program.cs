using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ServiceProcess;
using Utilities;
using System.Diagnostics;
using System.Threading;
using System.ServiceModel;
using System.Reflection;

namespace ParallelForEach
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            // WindowsServicesProcessingTest();
            StartUpdates();
            Console.ReadKey();
        }

        private const int poolTimeout = 30000;
        private static object timerLock = new object();
        private static BackgroundTimer updatesBackgroundTimer;
        public static int TotalCancelations { get; set; }

        private static readonly Random random = new Random(DateTime.Now.Second);

        private static void StartUpdates()
        {
            if (updatesBackgroundTimer == null)
            {
                // created but not yet started
                updatesBackgroundTimer = new BackgroundTimer(OnUpdateTimer, Timeout.Infinite, poolTimeout);
            }
            updatesBackgroundTimer.Start(poolTimeout);

        }

        private static void UpdatesInLongWindingProcess()
        {
            var cancellationTokenTimeout = poolTimeout;
            var cancellationTokenInstance = new CancellationTokenSource();
            ServiceController[] services = ServiceController.GetServices();
            var po = new ParallelOptions { CancellationToken = cancellationTokenInstance.Token, MaxDegreeOfParallelism = Environment.ProcessorCount };

            // The delegate to be executed when/if the CancellationToken is canceled.
            cancellationTokenInstance.Token.Register(
                (object state) =>
                {
                    // action if a cancellation on token occurs  
                    TotalCancelations++;
                    var token = (CancellationToken)state;
                    if (token.IsCancellationRequested)
                    {
                        Debug.WriteLine("!!! Cancellation Event in UpdatesInLongWindingProcess timeout of " + cancellationTokenTimeout);
                        Debug.WriteLine("!!! UpdatesInLongWindingProcess() Cancellation Event triggered after elapsed time, TotalCancellations={0}", TotalCancelations);

                    }
                },
                cancellationTokenInstance.Token);

            var signledDone = (ManualResetEvent)cancellationTokenInstance.Token.WaitHandle;
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                Thread.CurrentThread.Name = "UpdatesInLongWindingProcess";
               // ThreadHelpers.DisplayThreadPoolInfo(Thread.CurrentThread.Name);

                // The countdown for the millisecondsDelay starts during this call. 
                // When the millisecondsDelay expires, this CancellationTokenSource is canceled, if it has not been canceled already.
                if (cancellationTokenTimeout > 0)
                {
                    // schedule a cancel operation on the source
                    cancellationTokenInstance.CancelAfter(cancellationTokenTimeout + 1000); // 30000 ms

                    // start a thread to monitor the Parallel Loop process to verify it gets to the end. 
                    // That Loop should signal when done
                    Task.Factory.StartNew(
                        (object state) =>
                        {
                            Thread.CurrentThread.Name = "Monitor UpdatesInLongWindingProcess Thread";
                            var token = state as CancellationTokenSource;
                            if (signledDone.WaitOne(cancellationTokenTimeout))
                            {
                                TotalCancelations = 0;
                            }
                            else
                            {
                                Debug.WriteLine(":( Watchdog Thread timeout. UpdatePredictionTime process Failed to Signal finish of Bus Updates in time, Cancellation expected. TotalCancelations={0}", TotalCancelations);
                                if (token != null && !token.IsCancellationRequested)
                                {
                                    token.Cancel();
                                    Debug.WriteLine("Invoked Token.Cancel()...");
                                }
                            }
                        },
                        cancellationTokenInstance);
                }

                Parallel.ForEach(
                    services,
                    po,
                    service =>
                    {
                        var cancellationToken = po.CancellationToken;
                        var swForEach = new Stopwatch();
                        swForEach.Start();

                        // check if canceled, if true throw exception and trap below
#pragma warning disable RECS0065 // Expression is always 'true' or always 'false'
                        if (cancellationToken != null)
#pragma warning restore RECS0065 // Expression is always 'true' or always 'false'
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                        }

                        Task.Factory.StartNew(
                            (object state) =>
                            {
                               
                                Thread.CurrentThread.Name = $"Thread " + service.ServiceName;
                            //    ThreadHelpers.DisplayMemory(Thread.CurrentThread.Name);
                                int sleepThreadFor = random.Next(1000);
                                Thread.Sleep(sleepThreadFor); // each thread can take 30 seconds
                                LongWindingParentProcess(sleepThreadFor);
                                //Debug.WriteLine($"Threads Count: {Process.GetCurrentProcess().Threads.Count}, ThreadId:{Thread.CurrentThread.ManagedThreadId } - Service {service.ServiceName} - Sleep For {sleepThreadFor} ms" );
                            },
                            service);
                       
                });
               
            }
            catch (OperationCanceledException operationCanceledException)
            {
                Debug.WriteLine(operationCanceledException.Message);
            }
            catch (CommunicationException communicationException)
            {
                Debug.WriteLine( communicationException.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                try
                {
                    // Signal we are done  
                    Debug.WriteLine($" Elapsed Time {sw.ElapsedMilliseconds}");
                    sw.Restart();
                    signledDone.Set();
                    cancellationTokenInstance.Dispose();
                    ThreadHelpers.DisplayMemory("-----Finalaly ----");
                    ThreadHelpers.DisplayThreadPoolInfo("-------------Finally--------");
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
                {
                }
            }
        }

        public static void OnUpdateTimer(object state)
        {
            var sw = new Stopwatch();
            sw.Start();
            var locked = false;
            try
            {
                if ((locked = Monitor.TryEnter(timerLock, 1000)) == true)
                {
                    UpdatesInLongWindingProcess();
                }
            }
            catch (Exception e)
            {
                Console.Write("OnUpdatesInLongWindingProcessTimer() Unhanded Exception {0}", e.Message);
            }
            finally
            {
                if (locked)
                {
                    Monitor.Exit(timerLock);
                }
                sw.Stop();
            }
        }

        //private static Semaphore waitLockParent = new Semaphore(0, 1);
        private static object waitLockParent = new object();
        public static void LongWindingParentProcess(int sleep)
        {
            var gotThread = false;
            try
            {
              //  gotThread = waitLockParent.WaitOne(1000);
                lock (waitLockParent)
                {
                    LongWindingChildProcess(sleep);
                   // Thread.Sleep(sleep);
                    Debug.WriteLine($" in {MethodBase.GetCurrentMethod().Name}");
                }
            }
            finally
            {
                if (gotThread)
                {
                  //  waitLockParent.Release();
                    Debug.WriteLine($"Releasing lock in {MethodBase.GetCurrentMethod().Name}");
                }
                else
                {
                    Debug.WriteLine($"Holding the lock in {MethodBase.GetCurrentMethod().Name}");
                }
            }
        }
      // private static Semaphore waitLockChild = new Semaphore(0, 1);
        private static object waitLockChild = new object();
        private static void LongWindingChildProcess(int sleep)
        {
           
            var gotThread = false;
            try
            {
             //   gotThread = waitLockChild.WaitOne(1000);
                lock (waitLockChild)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        LongWindingChildOfChildProcess(sleep);
                    }
                    Debug.WriteLine($" in {MethodBase.GetCurrentMethod().Name}");
                }
            }
            finally
            {
                if (gotThread)
                {
                  //  waitLockChild.Release();
                    Debug.WriteLine($"Releasing lock in {MethodBase.GetCurrentMethod().Name}");
                }
                else
                {
                    Debug.WriteLine($"Holding the lock in {MethodBase.GetCurrentMethod().Name}");
                }
            }
        }
        // private static Semaphore waitLockChildOfChild = new Semaphore(0, 1);
          private static object waitLockChildOfChild = new object();
        private static void LongWindingChildOfChildProcess(int sleep)
        {
            var gotThread = false;
            try
            {
             //   gotThread = waitLockChildOfChild.WaitOne(1000);
                lock (waitLockChildOfChild)
                {
                   
                    Thread.Sleep(sleep);
                    Debug.WriteLine($" in {MethodBase.GetCurrentMethod().Name}");
                }
            }
            finally
            {
                if (gotThread)
                {
                    //waitLockChildOfChild.Release();
                    Debug.WriteLine($"Releasing lock in {MethodBase.GetCurrentMethod().Name}");
                }
                else
                {
                    Debug.WriteLine($"Holding the lock in {MethodBase.GetCurrentMethod().Name}");
                }
            }
        }

        private static void WindowsServicesProcessingTest()
        {
            ServiceController[] services = ServiceController.GetServices();

            Console.WriteLine(services.ReportAllProperties());
            var sw = Stopwatch.StartNew();
            PrintAllServices(services);
            Console.WriteLine($"PrintAllServices Took {sw.ElapsedMilliseconds}");
            Console.ReadKey();
            sw = Stopwatch.StartNew();
            PrintAllServicesParallelForEach(services);
            Console.WriteLine($"PrintAllServies Parallel Took {sw.ElapsedMilliseconds}");
            Console.ReadKey();
            sw = Stopwatch.StartNew();
            PrintAllServices(services);
            Console.WriteLine($"PrintAllServies Took {sw.ElapsedMilliseconds}");
            Console.ReadKey();
            sw = Stopwatch.StartNew();
            PrintAllServicesParallelForEach(services);
            Console.WriteLine($"PrintAllServies Parallel Took {sw.ElapsedMilliseconds}");

            Console.Write($"Final Thread Count {Process.GetCurrentProcess().Threads.Count} ");
        }

        private static void PrintAllServices(ServiceController[] services)
        {
            foreach (var item in services)
            {
                Console.Write($"{Process.GetCurrentProcess().Threads.Count} - {Thread.CurrentThread.ManagedThreadId}");
                // Console.WriteLine();
                Thread.Sleep(10);
            }
        }

        private static void PrintAllServicesParallelForEach(ServiceController[] services)
        {
            Parallel.ForEach(services, item =>
            {
                Console.WriteLine($" Thread Count: {Process.GetCurrentProcess().Threads.Count} - {Thread.CurrentThread.ManagedThreadId}");
                //Console.WriteLine();
                Thread.Sleep(10);
            });

        }
    }
}
