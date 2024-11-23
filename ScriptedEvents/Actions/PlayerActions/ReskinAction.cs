using System;
using Exiled.API.Extensions;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class ReskinAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Reskin";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets the appearance of all players to the given role, but does NOT change their role.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set the appearance of all the players as.", true),
            new Argument("targetPlayers", typeof(Player[]), "The players that will see reskin taking place. Do not provide this argument if all players are to see the reskin.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] plys = (Player[])Arguments[0]!;
            RoleTypeId roleType = (RoleTypeId)Arguments[1]!;

            if (Arguments.Length >= 3)
            {
                Player[] targetPlys = (Player[])Arguments[2]!;
                foreach (Player player in plys)
                    player.ChangeAppearance(roleType, targetPlys);

                return new(true);
            }

            foreach (Player player in plys)
                player.ChangeAppearance(roleType);

            return new(true);
        }
    }
}
