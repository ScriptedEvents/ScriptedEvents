﻿namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

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
