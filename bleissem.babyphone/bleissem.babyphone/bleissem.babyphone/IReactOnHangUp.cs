using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public interface IReactOnHangUp: IDisposable
    {
        void Accept(Action actionOnHangUp);
    }
}
