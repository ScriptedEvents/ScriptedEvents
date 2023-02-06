using Exiled.API.Features;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptedEvents.Actions
{
    public class BroadcastPlayerAction : IAction
    {
        public string Name => "BROADCASTPLAYER";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 2) return new(false, "Missing argument: players, duration, message");

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out List<Player> players))
            {
                return new(false, "No players matching the criteria were found.");
            }

            if (!ScriptHelper.TryConvertNumber(Arguments[1], out float duration))
            {
                return new(false, "Invalid duration provided!");
            }

            string message = string.Join(" ", Arguments.Skip(2).Select(arg => ConditionVariables.ReplaceVariables(arg)));
            foreach (Player player in players)
            {
                player.Broadcast((ushort)duration, message);
            }
            return new(true);
        }
    }
}