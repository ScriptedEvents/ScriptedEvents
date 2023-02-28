namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using UnityEngine;

    public class TpxAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPX";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Teleports players to the specified X, Y, Z coordinates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("X", typeof(float), "The X-coordinate to teleport to.", true),
            new Argument("Y", typeof(float), "The Y-coordinate to teleport to.", true),
            new Argument("Z", typeof(float), "The Z-coordinate to teleport to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] players))
                return new(MessageType.NoPlayersFound, this, "players");

            if (!float.TryParse(Arguments[1], out float x))
                return new(MessageType.NotANumber, this, "X", Arguments[1]);

            if (!float.TryParse(Arguments[2], out float y))
                return new(MessageType.NotANumber, this, "Y", Arguments[2]);

            if (!float.TryParse(Arguments[3], out float z))
                return new(MessageType.NotANumber, this, "Z", Arguments[3]);

            Vector3 vz = new(x, y, z);

            foreach (Player ply in players)
            {
                if (ply.IsDead || !ply.IsConnected) continue;
                ply.Teleport(vz);
            }

            return new(true);
        }
    }
}
