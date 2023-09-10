namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Roles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class TpSpawnAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPSPAWN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => $"Teleports players to the specified {nameof(SpawnLocationType)}.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("spawn", typeof(SpawnLocationType), "The spawn to teleport to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection players, script))
                return new(false, players.Message);

            if (!VariableSystem.TryParse(Arguments[1], out SpawnLocationType rt, script))
                return new(false, $"Invalid spawn: {Arguments[1]}. View all valid spawns at: https://exiled-team.github.io/EXILED/api/Exiled.API.Enums.SpawnLocationType.html");

            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                ply.Teleport(rt, UnityEngine.Vector3.up);
            }

            return new(true);
        }
    }
}
