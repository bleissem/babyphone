using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bleissem.babyphone
{
    public interface ISpeaker: IDisposable
    {
        void Turn(bool on, SettingsTable.CallTypeEnum calltype);
    }
}
