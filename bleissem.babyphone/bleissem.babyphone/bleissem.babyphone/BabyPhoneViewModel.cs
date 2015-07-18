﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public class BabyPhoneViewModel : IDisposable
    {
        #region constructor

        public BabyPhoneViewModel(IAudioRecorder audioRecorder, Settings settings, ICallNumber callNumber)
        {
            m_RecorderViewModel = audioRecorder;
            m_RecorderViewModel.Start();
            m_PhoneViewModel = new PhoneViewModel(m_RecorderViewModel, settings, callNumber);


            m_InfoTimer = new MyTimer(new TimeSpan(0, 0, 1));
            m_InfoTimer.AutoReset = true;
            m_InfoTimer.Elapsed += m_Timer_Elapsed;
            m_InfoTimer.Start();
        }

        #endregion

        ~BabyPhoneViewModel()
        {
            this.Dispose(false);
        }

        #region InfoTimer

        private MyTimer m_InfoTimer;

        void m_Timer_Elapsed(object sender, MyTimerElapsedEventArgs e)
        {
            if ((null != PeriodicNotifications) && (null != m_RecorderViewModel))
            {

                PeriodicNotifications(this, m_RecorderViewModel.GetAmplitude());
            }
        }

        public event EventHandler<int> PeriodicNotifications;

        #endregion

        private IAudioRecorder m_RecorderViewModel;

        private PhoneViewModel m_PhoneViewModel;

        public PhoneViewModel Phone
        {
            get
            {
                return m_PhoneViewModel;
            }
        }


        private void Dispose(bool disposing)
        {
            if (null != m_InfoTimer)
            {
                m_InfoTimer.Stop();
                m_InfoTimer.Elapsed -= m_Timer_Elapsed;
                m_InfoTimer.Dispose();
                m_InfoTimer = null;
            }

            if (null != m_RecorderViewModel)
            {
                m_RecorderViewModel.Dispose();
                m_RecorderViewModel = null;

            }

            if (null != m_PhoneViewModel)
            {
                m_PhoneViewModel.Dispose();
                m_PhoneViewModel = null;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
