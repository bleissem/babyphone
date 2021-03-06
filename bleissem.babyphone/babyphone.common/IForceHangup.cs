﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public interface IForceHangup : IDisposable
    {        
        void Hangup();
        void Register(Action forceHangupAction);
    }
}
