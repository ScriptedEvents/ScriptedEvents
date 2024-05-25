namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class SetRoleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SETROLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

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
            RoleTypeId roleType = (RoleTypeId)Arguments[1];
            PlayerCollection players = (PlayerCollection)Arguments[0];

            bool setSpawnpoint = Arguments.Length == 2 || (bool)Arguments[2];
            bool setInventory = Arguments.Length <= 3 || (bool)Arguments[3];

            RoleSpawnFlags flags = RoleSpawnFlags.None;

            if (setSpawnpoint)
                flags |= RoleSpawnFlags.UseSpawnpoint;

            if (setInventory)
                flags |= RoleSpawnFlags.AssignInventory;

            foreach (Player player in players)
                player.Role.Set(roleType, flags);

            return new(true);
        }
    }
}