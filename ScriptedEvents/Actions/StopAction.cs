using ScriptedEvents.Actions.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class StopAction : IScriptAction, IHelpInfo
    {
        public string Name => "STOP";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Stops the event execution at this line.";

        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        public ActionResponse Execute(Script script)
        {
            return new(true, flags: ActionFlags.StopEventExecution);
        }
    }
}
