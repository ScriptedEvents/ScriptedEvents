﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.IO;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class ExecuteScriptAction : IScriptAction, IHelpInfo
    {
        public string Name => "EXECUTESCRIPT";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Executes a different script.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("scriptName", typeof(string), "The name of the script.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
            string scriptName = Arguments[0];

            try
            {
                ScriptHelper.ReadAndRun(scriptName);
                return new(true);
            }
            catch (DisabledScriptException)
            {
                return new(false, $"Script '{scriptName}' is disabled.");
            }
            catch (FileNotFoundException)
            {
                return new(false, $"Script '{scriptName}' not found.");
            }
        }
    }
}
