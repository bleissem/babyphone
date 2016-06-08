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

        }

        #endregion

        ~AudioRecorderViewModel()
        {
            this.Dispose(false);
        }


        private Android.Media.AudioRecord m_AudioRecord;
        private int m_MinSize;
        private const int SampleRate = 8000;


        public void Start()
        {
            if (!IsStarted)
            {
                this.Initialize();
            }
        }

        private void Initialize()
        {
            m_MinSize = Android.Media.AudioRecord.GetMinBufferSize(SampleRate, Android.Media.ChannelIn.Mono, Android.Media.Encoding.Pcm16bit);
            m_AudioRecord = new Android.Media.AudioRecord(Android.Media.AudioSource.Mic, SampleRate, Android.Media.ChannelIn.Mono, Android.Media.Encoding.Pcm16bit, m_MinSize);
            m_AudioRecord.StartRecording();

        }


        public void Pause()
        {
            if (!this.IsStarted) return;

            try
            {
                m_AudioRecord.Stop();
            }
            catch
            {

            }
        }

        public void Resume()
        {
            if (!this.IsStarted) return;
            try
            {
                m_AudioRecord.StartRecording();
            }
            catch
            {

            }
        }


        public void Stop()
        {
            CleanUp();
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
            if (!IsStarted)
            {
                return 0;
            }

            short[] buffer = new short[m_MinSize];
            m_AudioRecord.Read(buffer, 0, m_MinSize);
            int max = 0;
            foreach (short s in buffer)
            {
                if (Math.Abs(s) > max)
                {
                    max = Math.Abs(s);
                }
            }
            return max;
        }


        private void CleanUp()
        {
            if (null != m_AudioRecord)
            {
                m_AudioRecord.Stop();
                m_AudioRecord.Release();
                m_AudioRecord.Dispose();
                m_AudioRecord = null;
            }
        }

        private void Dispose(bool disposing)
        {
            CleanUp();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
