using System;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class SetRoleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SetRole";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets all players to the given role with advanced settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set all the players as.", true),
            new Argument("spawnpoint", typeof(bool), "Use spawnpoint? default: true", false),
            new Argument("inventory", typeof(bool), "Use default inventory? default: true", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            RoleTypeId roleType = (RoleTypeId)Arguments[1]!;
            PlayerCollection players = (PlayerCollection)Arguments[0]!;
            RoleSpawnFlags flags = RoleSpawnFlags.None;

            if ((bool?)Arguments[2] ?? true)
                flags |= RoleSpawnFlags.UseSpawnpoint;

            if ((bool?)Arguments[3] ?? true)
                flags |= RoleSpawnFlags.AssignInventory;

            foreach (Player player in players)
                player.Role.Set(roleType, flags);

            return new(true);
        }
    }
}