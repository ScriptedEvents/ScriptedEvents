using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using System;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
    public class StartRoundAction : IScriptAction, IHelpInfo
    {
        public string Name => "START";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Starts the round.";

        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        public ActionResponse Execute(Script scr)
        {
            Round.Start();
            return new(true);
        }
    }
}