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
using Android.Media;

namespace bleissem.babyphone.Droid
{
    public class Speaker : ISpeaker
    {
        public Speaker(AudioManager audioManager)
        {
            m_AudioManager = audioManager;
        }

        ~Speaker()
        {
            Dispose(false);
        }

        private AudioManager m_AudioManager;

        private void Dispose(bool dispose)
        {
            m_AudioManager = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Turn(bool on, SettingsTable.CallTypeEnum calltype)
        {
            if (null == m_AudioManager) return;

            if (calltype == SettingsTable.CallTypeEnum.Phone)
            {
                m_AudioManager.Mode = Mode.InCall;
            }
            else
            {
                m_AudioManager.Mode = Mode.InCommunication;
            }
            m_AudioManager.SpeakerphoneOn = on;
        }

    }
}