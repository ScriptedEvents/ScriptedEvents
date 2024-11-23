using Exiled.API.Features;
using Exiled.API.Features.Roles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Teleportation
{
    public class TpPlayerAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TPPlayer";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "TPPlr" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Teleportation;

        /// <inheritdoc/>
        public string Description => "Teleports players to another (only one!) player.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to teleport", true),
            new Argument("targetPlayer", typeof(Player), "The player to teleport to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] players = (Player[])Arguments[0]!;
            Player targetPlayer = (Player)Arguments[1]!;

            foreach (Player ply in players)
            {
                if (ply.Role is not FpcRole || !ply.IsConnected) continue;
                ply.Teleport(targetPlayer.Position);
            }

            return new(true);
        }
    }
}
