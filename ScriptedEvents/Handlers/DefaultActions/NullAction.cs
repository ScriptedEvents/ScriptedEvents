using ScriptedEvents.API.Features.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.DefaultActions
{
    // Represents a line in a file that does not have any actions.
    public class NullAction : IAction, IHiddenAction
    {
        public string Name => "NULL";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }
    }
}
