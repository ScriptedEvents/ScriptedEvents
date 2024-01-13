namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class CommandAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "COMMAND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Runs a server command with full permission. Variables are supported.";

        /// <inheritdoc/>
        public string LongDescription => @"This action executes commands as the server. Therefore, the command needs '/' before it if it's a RA command, or '.' before it if its a console command.
Note: Player variables will not work as expected. In order to use player variables, encase them within the 'C' variable. For example: 'COMMAND /kill {C:PLAYERS}' to kill all players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("command", typeof(string), "The command to run.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string text = VariableSystem.ReplaceVariables(string.Join(" ", Arguments), script);
            GameCore.Console.singleton.TypeCommand(text);
            return new(true, string.Empty);
        }
    }
}
