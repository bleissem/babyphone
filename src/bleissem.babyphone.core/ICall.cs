using System;
using System.Collections.Generic;
using System.Text;

namespace bleissem.babyphone.Core
{
    public interface ICall : IDisposable
    {
        void Number();
    }
}