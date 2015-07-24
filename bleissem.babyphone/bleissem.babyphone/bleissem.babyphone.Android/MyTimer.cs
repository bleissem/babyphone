using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.Droid
{
    public class MyTimer:  System.Timers.Timer, ITimer
    {

        #region constructor 

        public MyTimer(TimeSpan timespan)
        {
            base.Interval = timespan.TotalMilliseconds;
            base.Elapsed += MyTimer_Elapsed;
        }

        #endregion

        private void MyTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (null != MyElapsed)
            {
                MyElapsed(sender, new MyTimerElapsedEventArgs() { SignalTime = e.SignalTime });
            }
        }

        public event MyTimerElapsedEventArgs.OnElapsed MyElapsed;


    }


    public class MyTimerCreator: ICreateTimer
    {

        public ITimer Create(TimeSpan timespan)
        {
            return new MyTimer(timespan);
        }
    }
}
