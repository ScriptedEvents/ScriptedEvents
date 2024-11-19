using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Server
{
    public class CommandAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "Command";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Server;

        /// <inheritdoc/>
        public string Description => "Runs a server command with FULL PERMISSION(!!!).";

        /// <inheritdoc/>
        public string LongDescription => @"Some common confusions with this action:

This action executes commands as the server. Therefore, the command needs '/' before it if you want to executen an RA command, or '.' before it you want to execute a console command.

Player variables dont work here. If you want to specify players, you will have to do that using player id's instead.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("command", typeof(string), "The command to run.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            GameCore.Console.singleton.TypeCommand(Arguments.JoinMessage());
            return new(true);
        }
    }
}
