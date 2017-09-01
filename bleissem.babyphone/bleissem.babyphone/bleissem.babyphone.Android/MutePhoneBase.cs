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
    public class MutePhoneBase : IDisposable
    {
        public MutePhoneBase(AudioManager audioManager)
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

            m_BackupStreams = new SortedDictionary<Stream, int>();
            this.BackUp();           
        }

        ~MutePhoneBase()
        {
            this.Dispose(false);
        }

        private AudioManager m_AudioManager { get; set; }
        private IEnumerable<Stream> m_Streams;
        SortedDictionary<Stream, int> m_BackupStreams;

        private void Dispose(bool disposing)
        {
            m_AudioManager = null;
        }

        public void Dispose()
        {
            this.Dispose(true);
        }

        private void BackUp()
        {
            foreach(Stream stream in this.m_Streams)
            {
                m_BackupStreams.Add(stream, m_AudioManager.GetStreamVolume(stream));
            }
        }

        protected void Mute()
        {
            foreach (Stream stream in this.m_Streams)
            {
                this.m_AudioManager.AdjustStreamVolume(stream, Adjust.Mute, VolumeNotificationFlags.RemoveSoundAndVibrate);
            }
        }
       
        protected void UnMute()
        {
            foreach (Stream stream in this.m_Streams)
            {
                this.m_AudioManager.AdjustStreamVolume(stream, Adjust.Unmute, VolumeNotificationFlags.AllowRingerModes);
                this.m_AudioManager.SetStreamVolume(stream, m_BackupStreams[stream], VolumeNotificationFlags.AllowRingerModes);
            }
        }
    }
}