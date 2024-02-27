namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class StorageAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "STORAGE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Variable;

        /// <inheritdoc/>
        public string Description => ".............";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("variableName", typeof(string), "The name of the new variable. Braces will be added automatically if not provided.", false),
            new Argument("value", typeof(object), "The value to store. Variables & Math are supported.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            VariableStorage.Save("test", "testststtsys");
            return new(true);
        }
    }
}
