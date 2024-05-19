namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

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
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

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
            string message = VariableSystemV2.ReplaceVariables(Arguments.JoinMessage(0), script);

            if (script.Sender is null || script.Context is ExecuteContext.Automatic)
            {
                Log.Info(message);
                return new(true);
            }

            if (script.Context is ExecuteContext.PlayerConsole)
            {
                Player.Get(script.Sender)?.SendConsoleMessage(message, "green");
                return new(true);
            }

            script.Sender.Respond(message);

            return new(true);
        }
    }
}