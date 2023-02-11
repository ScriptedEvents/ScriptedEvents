using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.API.Helpers;
using System;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
    public class DebugConditionLogAction : IScriptAction, IHiddenAction
    {
        public string Name => "DEBUGCONDITIONLOG";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute(Script script)
        {
            Log.Info(ConditionHelper.Evaluate(string.Join("", Arguments)));
            return new(true);
        }
    }
}