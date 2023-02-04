using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class DeletePlayerVariable : IAction
    {
        public string Name => "DELPLAYERVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing argument: variableName");
            if (PlayerVariables.Variables.ContainsKey(Arguments[0]))
            {
                PlayerVariables.Variables.Remove(Arguments[0]);
                return new(true);
            }

            return new(false, $"Invalid variable '{Arguments[0]}'");
        }
    }
}
