using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bleissem.babyphone.Droid
{
    public class AudioRecorderViewModel : IAudioRecorder
    {

        #region Constructor

        public AudioRecorderViewModel()
        {
            m_MinSize = Android.Media.AudioRecord.GetMinBufferSize(SampleRate, Channelin, MediaEncoding);
        }

        #endregion

        ~AudioRecorderViewModel()
        {
            this.Dispose(false);
        }


        private volatile Android.Media.AudioRecord m_AudioRecord;
        private int m_MinSize;
        private const int SampleRate = 8000;
        private const Android.Media.ChannelIn Channelin = Android.Media.ChannelIn.Mono;
        private const Android.Media.Encoding MediaEncoding = Android.Media.Encoding.Pcm16bit;

        private Android.Media.AudioSource MediaAudioSource
        {
            get
            {
                int androidSDKVersion = 0;


                if (Int32.TryParse(Android.OS.Build.VERSION.Sdk, out androidSDKVersion) && (androidSDKVersion < 11))
                {
                    return Android.Media.AudioSource.Mic;                    
                }                
                return Android.Media.AudioSource.VoiceRecognition;
            }
        }

        private object m_LockObj = new Object();


        public void Start()
        {
            lock (m_LockObj)
            {
                this.Initialize();
            }
        }

        private void Initialize()
        {
            if (IsStarted) return;
            
            try
            {
                m_AudioRecord = new Android.Media.AudioRecord(this.MediaAudioSource, SampleRate, Channelin, MediaEncoding, m_MinSize);
                m_AudioRecord.StartRecording();
            }
            catch
            {

            }
        }

        public void Stop()
        {
            lock (m_LockObj)
            {
                CleanUp();
            }
        }

        public bool IsStarted
        {
            get
            {
                return (null != m_AudioRecord);
            }
        }

        public int GetAmplitude()
        {
            lock (m_LockObj)
            {
                if (!IsStarted)
                {
                    return 0;
                }

                short[] buffer = new short[m_MinSize];
                m_AudioRecord.Read(buffer, 0, m_MinSize);
                int max = 0;
                int ints = 0;
                foreach (short s in buffer)
                {
                    ints = Math.Abs(Convert.ToInt32(s));
                    if (ints > max)
                    {
                        max = ints;
                    }
                }

                if (0 == max)
                {
                    // this might happen
                    this.CleanUp();
                    this.Initialize();
                }

                return max;
            }
        }


        private void CleanUp()
        {
            if (null != m_AudioRecord)
            {
                try
                {
                    m_AudioRecord.Stop();
                    m_AudioRecord.Release();
                    m_AudioRecord.Dispose();
                }
                catch
                {

                }
                m_AudioRecord = null;
            }
        }

        private void Dispose(bool disposing)
        {
            CleanUp();
        }

        private bool m_SuppressFinalizeThis = true;

        public void Dispose()
        {
            bool alreadyDisposed = null == m_AudioRecord;
            this.Dispose(true);
            if (m_SuppressFinalizeThis)
            {
                m_SuppressFinalizeThis = false;
                GC.SuppressFinalize(this);
            }
        }
    }
}
