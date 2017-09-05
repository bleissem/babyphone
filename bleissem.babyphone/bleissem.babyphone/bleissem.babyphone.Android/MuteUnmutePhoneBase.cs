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
    public class MuteUnmutePhoneBase : IDisposable, IUnMutePhone, IMutePhone
    {
        public MuteUnmutePhoneBase(AudioManager audioManager)
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
        }

        ~MuteUnmutePhoneBase()
        {
            this.Dispose(false);
        }

        private AudioManager m_AudioManager { get; set; }
        private IEnumerable<Stream> m_Streams;

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
            if (null == m_AudioManager) return;

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