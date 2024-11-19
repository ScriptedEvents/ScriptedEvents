using System;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Server
{
    public class ErrorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Error";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Server;

        /// <inheritdoc/>
        public string Description => "Creates an error message in the server console.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Logger.ScriptError(Arguments.JoinMessage(), script, true);
            return new(true);
        }
    }
}