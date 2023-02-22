namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using UnityEngine;

    public class TpRoomAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPROOM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified room center.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("room", typeof(RoomType), "The room to teleport to. Do NOT use Scp173 room!!!", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] players))
                return new(MessageType.NoPlayersFound, this, "players");

            if (!Enum.TryParse(Arguments[1], out RoomType rt))
                return new(false, $"Invalid room: {Arguments[1]}");

            foreach (Player ply in players)
            {
                if (ply.IsDead || !ply.IsConnected) continue;
                ply.Teleport(rt);
            }

            return new(true);
        }
    }
}
