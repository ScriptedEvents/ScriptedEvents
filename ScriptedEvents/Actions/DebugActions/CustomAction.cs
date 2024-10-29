namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class CustomAction : IScriptAction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomAction"/> class.
        /// </summary>
        /// <param name="name">The name of the custom action.</param>
        /// <param name="action">The function to execute when the action is invoked.</param>
        public CustomAction(string name, Func<Tuple<string[], object>, Tuple<bool, string, object[]>> action)
        {
            Name = name;
            Action = action;
        }

        /// <inheritdoc/>
        public string Name { get; } = "UNKNOWN";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <summary>
        /// Gets the <see cref="Func{T, TResult}"/> to execute when this action is executed.
        /// </summary>
        public Func<Tuple<string[], object>, Tuple<bool, string, object[]>> Action { get; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var result = Action(new(RawArguments, script));
            return new(result.Item1, variablesToRet: result.Item3);
        }
    }
}
