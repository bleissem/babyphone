using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public interface IAudioRecorder: IDisposable
    {

        void Start();
        void Stop();
        void Pause();
        void Resume();

        bool IsStarted
        {
            get;
        }

        int GetAmplitude();
    }
}
