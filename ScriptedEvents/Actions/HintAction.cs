namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class HintAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HINT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Broadcast;

        /// <inheritdoc/>
        public string Description => "Broadcasts a hint to every player.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("duration", typeof(float), "The duration of the message. Variables & Math are NOT supported.", true),
            new Argument("message", typeof(string), "The message. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!float.TryParse(Arguments[0], out float duration))
            {
                return new(MessageType.NotANumber, this, "duration", Arguments[0]);
            }

            string message = string.Join(" ", Arguments.Skip(1).Select(arg => VariableSystem.ReplaceVariables(arg, script)));
            Map.ShowHint(message, duration);
            return new(true);
        }
    }
}