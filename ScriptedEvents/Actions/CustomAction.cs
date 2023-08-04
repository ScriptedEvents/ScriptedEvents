﻿namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class CustomAction : IScriptAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAction"/> class.
        /// </summary>
        /// <param name="name">The name of the custom action.</param>
        /// <param name="action">The function to execute when the action is invoked.</param>
        public CustomAction(string name, Func<string[], Tuple<bool, string>> action)
        {
            Name = name;
            Action = action;
        }

        /// <inheritdoc/>
        public string Name { get; } = "UNKNOWN";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <summary>
        /// Gets the <see cref="Func{T, TResult}"/> to execute when this action is executed.
        /// </summary>
        public Func<string[], Tuple<bool, string>> Action { get; }

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Tuple<bool, string> result = Action(Arguments);
            return new(result.Item1, result.Item2);
        }
    }
}
