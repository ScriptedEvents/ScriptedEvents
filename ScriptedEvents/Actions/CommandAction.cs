namespace ScriptedEvents.Actions
{
    using System;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class CommandAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "COMMAND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Runs a server command with full permission.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("command", typeof(string), "The command to run.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string text = string.Join(" ", Arguments);
            GameCore.Console.singleton.TypeCommand(text);
            return new(true, string.Empty);
        }
    }
}
