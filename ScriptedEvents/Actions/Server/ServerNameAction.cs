using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    public class ServerNameAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SERVERNAME";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Server;

        /// <inheritdoc/>
        public string Description => "Sets this server's name.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("name", typeof(string), "The name to apply.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Server.Name = Arguments.JoinMessage();
            return new(true);
        }
    }
}
