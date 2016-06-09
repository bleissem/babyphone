using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace bleissem.babyphone.Droid
{
    public class TurnOnOffScreenViewModel: ITurnOnOffScreen
    {
        public TurnOnOffScreenViewModel(IExecuteAction turnOff, IExecuteAction turnOn)
        {
            m_TurnOff = turnOff;
            m_TurnOn = turnOn;            
        }

        ~TurnOnOffScreenViewModel()
        {
            this.Dispose(false);
        }

        private IExecuteAction m_TurnOn;
        private IExecuteAction m_TurnOff;

        public void TurnOff()
        {
            m_TurnOff.Execute();             
        }

        public void TurnOn()
        {
            m_TurnOn.Execute();
        }

        private void CleanUpWakeLock()
        {
            if (null != m_TurnOn)
            {
                try
                {
                    m_TurnOn.Dispose();
                }
                catch
                {

                }

                m_TurnOn = null;
            }

            if (null != m_TurnOff)
            {
                try
                {
                    m_TurnOff.Dispose();
                }
                catch
                {

                }

                m_TurnOff = null;
            }
        }

        private void Dispose(bool disposing)
        {
            this.CleanUpWakeLock();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}