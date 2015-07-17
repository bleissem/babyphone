using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class MyTimer
    {
        #region constructor

        private MyTimer()
        {
            this.AutoReset = false;            

        }

        public MyTimer(TimeSpan interval):this()
        {
            this.m_Interval = interval;
        }

        #endregion

        public delegate void ElapsedEventHandler(object sender, MyTimerElapsedEventArgs e);

        public event ElapsedEventHandler Elapsed;

        public bool AutoReset { get; set; }

        private TimeSpan m_Interval;
        private CancellationTokenSource m_CancelToken;
        private bool m_DoAbort;
        private object m_LockObj = "lock";

        public async void Start()
        {
            lock (m_LockObj)
            {
                this.m_CancelToken = new CancellationTokenSource();
            }

           await Task.Factory.StartNew(()=>
                {
                    m_DoAbort = false;

                    do
                    {
                        lock (m_LockObj)
                        {
                            if (this.m_CancelToken.IsCancellationRequested)
                            {
                                m_DoAbort = true;
                                return;
                            }
                        }

                        Task.Delay(m_Interval, m_CancelToken.Token).ContinueWith((t) =>
                            {
                                lock (m_LockObj)
                                {
                                    if (this.m_CancelToken.IsCancellationRequested)
                                    {
                                        m_DoAbort = true;
                                        return;
                                    }
                                }

                                if (null != Elapsed)
                                {
                                    Elapsed(this, new MyTimerElapsedEventArgs());
                                }
                            });
                    }
                    while (this.AutoReset && (!m_DoAbort));
                
                });
            
        }

        public void Stop()
        {
            lock(m_LockObj)
            {
                if (null != this.m_CancelToken)
                {
                    this.m_CancelToken.Cancel();
                }
            }
        }
    }

    public class MyTimerElapsedEventArgs
    {

        #region constructor

        public MyTimerElapsedEventArgs()
        {
            this.SignalTime = DateTime.Now;
        }

        #endregion
        public DateTime SignalTime { get; set; }
    }
}
