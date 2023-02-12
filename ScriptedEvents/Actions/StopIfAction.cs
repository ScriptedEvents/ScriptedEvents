﻿namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class StopIfAction : IScriptAction, IHelpInfo
    {
        public string Name => "STOPIF";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Reads the condition and stops execution of the script if the result is TRUE.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        public ActionResponse Execute(Script scr)
        {
            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join(string.Empty, Arguments));
            if (!outcome.Success)
                return new(false, $"STOPIF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (outcome.Passed)
                return new(true, flags: ActionFlags.StopEventExecution);

            return new(true);
        }
    }
}