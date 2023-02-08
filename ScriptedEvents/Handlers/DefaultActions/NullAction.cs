using ScriptedEvents.API.Features.Actions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.DefaultActions
{
    // Represents a line in a file that does not have any actions.
    public class NullAction : IAction
    {
        public string Name => "NULL";

        public string[] Aliases => throw new NotImplementedException();

        public string[] Arguments { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
