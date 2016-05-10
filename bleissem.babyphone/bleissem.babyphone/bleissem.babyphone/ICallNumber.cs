using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public interface ICallNumber: IDisposable
    {

        bool CanDial();
        void Dial();
        void Register(Action callAction, Func<bool> canCall);

    }
}
