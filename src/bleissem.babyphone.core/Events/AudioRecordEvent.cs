using bleissem.babyphone.Core.Messages;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace bleissem.babyphone.Core.Events
{
    public class AudioRecordEvent: PubSubEvent<AudioRecordMessage>
    {
    }
}
