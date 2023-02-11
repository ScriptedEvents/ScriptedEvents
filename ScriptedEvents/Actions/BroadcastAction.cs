using Exiled.API.Features;
using ScriptedEvents.Actions.Interfaces;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Variables;
using System;
using System.Linq;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions
{
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
            if (Arguments.Length < 2) return new(false, "Missing argument: duration, message");

            if (!ScriptHelper.TryConvertNumber(Arguments[0], out float duration))
            {
                return new(false, "Invalid duration provided!");
            }

            string message = string.Join(" ", Arguments.Skip(1).Select(arg => ConditionVariables.ReplaceVariables(arg)));
            Map.Broadcast((ushort)duration, message);
            return new(true);
        }
    }
}