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
            m_PreviousSpeakerOnOff = false;
            m_HasBeenTurnedOff = false;
        }

        ~Speaker()
        {
            Dispose(false);
        }

        private AudioManager m_AudioManager;
        private bool m_PreviousSpeakerOnOff;
        private bool m_HasBeenTurnedOff;

        private void Dispose(bool dispose)
        {
            m_AudioManager = null;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void TurnOff()
        {
            if (null == m_AudioManager) return;

            m_AudioManager.SpeakerphoneOn = m_PreviousSpeakerOnOff;
        }

        public void TurnOn()
        {
            if ((!m_HasBeenTurnedOff) || (null == m_AudioManager)) return;

            m_PreviousSpeakerOnOff = m_AudioManager.SpeakerphoneOn;
            m_AudioManager.Mode = Mode.InCall;
            m_AudioManager.SpeakerphoneOn = true;
        }
    }
}