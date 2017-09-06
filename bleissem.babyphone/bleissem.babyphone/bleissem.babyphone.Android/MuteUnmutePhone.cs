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
    public class MuteUnmutePhone : IDisposable, IUnMutePhone, IMutePhone
    {
        public MuteUnmutePhone(AudioManager audioManager)
        {
            m_AudioManager = audioManager;
            this.m_Streams = new List<Stream>()
            {
                Stream.Ring,
                Stream.Alarm,
                Stream.System,
                Stream.Music,
                Stream.Notification
            };

            m_HasBeenMuted = false;
        }

        ~MuteUnmutePhone()
        {
            this.Dispose(false);
        }

        private AudioManager m_AudioManager { get; set; }
        private IEnumerable<Stream> m_Streams;
        private bool m_HasBeenMuted;

        private void Dispose(bool disposing)
        {
            m_AudioManager = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }
        
        public void Mute()
        {
            if (null == m_AudioManager) return;
            m_HasBeenMuted = true;

            foreach (Stream stream in this.m_Streams)
            {
                try
                {
                    this.m_AudioManager.AdjustStreamVolume(stream, Adjust.Mute, VolumeNotificationFlags.RemoveSoundAndVibrate);
                }
                catch
                {

                }
            }
        }
       
        public void UnMute()
        {
            if ((!m_HasBeenMuted) || (null == m_AudioManager)) return;
            m_HasBeenMuted = false;

            foreach (Stream stream in this.m_Streams)
            {
                try
                {
                    this.m_AudioManager.AdjustStreamVolume(stream, Adjust.Unmute, VolumeNotificationFlags.AllowRingerModes);
                }
                catch
                {

                }
            }
        }
    }
}