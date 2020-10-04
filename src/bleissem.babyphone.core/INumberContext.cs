using System;
using System.Collections.Generic;
using System.Text;

namespace bleissem.babyphone.Core
{
    public interface INumberContext : IDisposable
    {
        void Register(string number);

        string Number { get; }
    }
}