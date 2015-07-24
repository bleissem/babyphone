using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class MyTimerElapsedEventArgs
    {

        #region constructor

        public MyTimerElapsedEventArgs()
        {
            this.SignalTime = DateTime.Now;
        }

        #endregion
        public DateTime SignalTime { get; set; }


        public delegate void OnElapsed(object sender, MyTimerElapsedEventArgs args);
    }

    public interface ITimer: IDisposable
    {
        bool AutoReset { get; set; }

        double Interval { get; set; }


        event MyTimerElapsedEventArgs.OnElapsed MyElapsed;
       
        void Start();
     
        void Stop();
    }

    

    public interface ICreateTimer
    {
        ITimer Create(TimeSpan timespan);
    }

}
