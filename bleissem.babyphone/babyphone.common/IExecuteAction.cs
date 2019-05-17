using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public interface IExecuteAction: IDisposable
    {
        void Execute();
    }

    public interface IExecuteGenericAction<T>: IDisposable
    {
        void SetValue(T value);
        void Execute();

    }
}
