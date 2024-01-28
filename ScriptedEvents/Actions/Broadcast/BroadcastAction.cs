namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class BroadcastAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "BROADCAST";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a message to every player.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("duration", typeof(float), "The duration of the message. Variables are supported.", true),
            new Argument("message", typeof(string), "The message. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            float duration = (float)Arguments[0];

            string message = VariableSystem.ReplaceVariables(Arguments.JoinMessage(1), script);
            Map.Broadcast((ushort)duration, message);
            return new(true);
        }
    }
}