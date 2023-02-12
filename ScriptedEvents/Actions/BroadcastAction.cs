namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class BroadcastAction : IScriptAction, IHelpInfo
    {
        public string Name => "BROADCAST";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Broadcasts a message to every player.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("duration", typeof(float), "The duration of the message. Variables & Math are NOT supported.", true),
            new Argument("message", typeof(string), "The message. Variables are supported.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!float.TryParse(Arguments[0], out float duration))
            {
                return new(MessageType.NotANumber, this, "duration", Arguments[0]);
            }

            string message = string.Join(" ", Arguments.Skip(1).Select(arg => ConditionVariables.ReplaceVariables(arg)));
            Map.Broadcast((ushort)duration, message);
            return new(true);
        }
    }
}