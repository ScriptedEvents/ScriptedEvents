using System;
using Exiled.API.Features;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Server
{
    public class PrintAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "PRINT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Server;

        /// <inheritdoc/>
        public string Description => "Creates a log message in the console the script was executed from.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string message = Arguments.JoinMessage(0);

            if (script.Sender is null || script.Context is ExecuteContext.Automatic)
            {
                Log.Info(message);
                return new(true);
            }

            if (script.Context is ExecuteContext.PlayerConsole)
            {
                Exiled.API.Features.Player.Get(script.Sender)?.SendConsoleMessage(message, "green");
                return new(true);
            }

            script.Sender.Respond(message);

            return new(true);
        }
    }
}