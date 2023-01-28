using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class BroadcastAction : IAction
    {
        public string Name => "BROADCAST";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
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