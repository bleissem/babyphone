using System;
using System.Collections.Generic;
using System.Text;

namespace bleissem.babyphone.Core
{
    public class NumberContext : INumberContext
    {
        public NumberContext()
        {
        }

        ~NumberContext()
        {
            Dispose(false);
        }

        public string Number { get; private set; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Register(string number)
        {
            Number = number;
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}