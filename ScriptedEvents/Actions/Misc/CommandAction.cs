namespace ScriptedEvents.Actions
{
    using System;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class CommandAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "COMMAND";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public string Description => "Runs a server command with full permission.";

        /// <inheritdoc/>
        public string LongDescription => @"This action executes commands as the server. Therefore, the command needs '/' before it if it's a RA command, or '.' before it if its a console command.
Note: Player variables will be converted to the amount of players when used directly. In order to use player variables to target players within a command, encase them within the 'C' variable. For example: 'COMMAND /kill {C:PLAYERS}' to kill all players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("command", typeof(string), "The command to run.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string text = VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(0), script);
            GameCore.Console.singleton.TypeCommand(text);
            return new(true, string.Empty);
        }
    }
}
