﻿using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.Handlers.Variables;
using System;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class DeletePlayerVariable : IScriptAction, IHelpInfo
    {
        public string Name => "DELPLAYERVARIABLE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Deletes a previously-defined player variable.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the variable.", true),
        };

        public ActionResponse Execute(Script script)
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
