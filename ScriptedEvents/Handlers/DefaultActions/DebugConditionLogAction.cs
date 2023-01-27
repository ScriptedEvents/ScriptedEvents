using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class DebugConditionLogAction : IAction
    {
        public string Name => "DEBUGCONDITIONLOG";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            Log.Info(ConditionHelper.Evaluate(string.Join("", Arguments)));
            return new(true);
        }
    }
}