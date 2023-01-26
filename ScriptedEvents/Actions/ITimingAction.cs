using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public interface ITimingAction : IAction
    {
        public float GetDelay(out ActionResponse message);
    }
}
