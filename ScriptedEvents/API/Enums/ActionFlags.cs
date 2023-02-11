using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.API.Enums
{
    [Flags]
    public enum ActionFlags
    {
        None = 0,
        FatalError = 1,
        StopEventExecution,
    }
}
