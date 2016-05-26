using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public enum CallStateEnum
    {
        Ringing,
        Offhook,
        Idle
    }


    public interface IReactOnCall: IDisposable
    {
        PhoneState State { get; }

        void Register(Action<CallStateEnum> onPhoneStateChange);
        void Register(Action onHangUp);

    }
}
