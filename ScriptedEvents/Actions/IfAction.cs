﻿namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class IfAction : IScriptAction, ILogicAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "IF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Reads the condition and stops execution of the script if the result is FALSE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("condition", typeof(string), "The condition to check. Variables & Math are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            ConditionResponse outcome = ConditionHelper.Evaluate(string.Join(string.Empty, Arguments));
            if (!outcome.Success)
                return new(false, $"IF execution error: {outcome.Message}", ActionFlags.FatalError);

            if (!outcome.Passed)
                return new(true, flags: ActionFlags.StopEventExecution);

            return new(true);
        }
    }
}