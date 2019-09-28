using System;
using System.Configuration;
using System.Diagnostics;
using System.Threading;

namespace ParallelForEach
{
   
        /// <summary>The background timer.</summary>
        public class BackgroundTimer
        {
            #region Constructors and Destructors

            /// <summary>Initializes a new instance of the <see cref="BackgroundTimer"/> class.</summary>
            /// <param name="callback">The callback. The method specified for callback should be reentrant, because it is called on ThreadPool threads. </param>
            /// <param name="dueTime">The amount of time to delay before callback is invoked, in milliseconds. Specify Timeout.Infinite to prevent the timer from starting. Specify zero (0) to start the timer immediately.</param>
            /// <param name="period">The time interval between invocations of callback, in milliseconds. Specify Timeout.Infinite to disable periodic signaling.</param>
            /// <exception cref="ArgumentNullException">Invalid callback</exception>
            public BackgroundTimer(TimerCallback callback, int dueTime, int period)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException(nameof(callback));
                }

                Debug.WriteLine("BackgroundTimer dueTime={0}, period={1}", dueTime, period);
                this.Timer = new Timer(callback, null, dueTime, period);
            }

            /// <summary>Initializes a new instance of the <see cref="BackgroundTimer"/> class.</summary>
            /// <param name="callback">The callback.</param>
            /// <param name="dueTime">The due time.</param>
            /// <param name="period">The period.</param>
            /// <exception cref="ArgumentNullException"></exception>
            public BackgroundTimer(TimerCallback callback, TimeSpan dueTime, TimeSpan period)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException(nameof(callback));
                }

                Debug.WriteLine("BackgroundTimer dueTime={0}, period={1}", dueTime, period);
                this.Timer = new Timer(callback, null, dueTime, period);
            }

            /// <summary>Initializes a new instance of the <see cref="BackgroundTimer"/> class. Call Start() to kick of the timer</summary>
            /// <param name="callback">The callback. The method specified for callback should be reentrant, because it is called on ThreadPool threads. </param>
            /// <exception cref="ArgumentNullException">Invalid callback</exception>
            public BackgroundTimer(TimerCallback callback)
            {
                if (callback == null)
                {
                    throw new ArgumentNullException(nameof(callback));
                }

                // create the timer but do not start it
                this.Timer = new Timer(callback, null, Timeout.Infinite, 0);
            }

            #endregion

            #region Public Properties

            /// <summary>Gets or sets the on timer callback.</summary>
            public TimerCallback OnTimerCallback { get; set; }

            /// <summary>Gets the timer.</summary>
            public Timer Timer { get; private set; }

            #endregion

            #region Public Methods and Operators

            /// <summary>Kill the Timer</summary>
            public void Kill()
            {
                if (this.Timer != null)
                {
                    this.Timer.Dispose();
                    this.Timer = null;
                }
            }

            /// <summary>Start the Timer</summary>
            /// <param name="period">The timer dueTime. interval</param>
            public void Start(int period)
            {
                Debug.Assert(this.Timer != null, "Invalid Timer");
                if (this.Timer != null)
                {
                    if (period < 1000)
                    {
                        period *= 1000; // to ms form minutes
                    }

                    Trace.TraceInformation("Start()  Period {0}", period);
                    this.Timer.Change(0, period);
                }
            }

            /// <summary>Stop the Timer</summary>
            public void Stop()
            {
                Debug.Assert(this.Timer != null, "Invalid Timer");
                if (this.Timer != null)
                {
                    Trace.TraceInformation("Stop Timer()");
                    this.Timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
            }

            #endregion
        }
    
}
