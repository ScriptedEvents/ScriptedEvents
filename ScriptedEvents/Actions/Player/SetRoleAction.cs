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
        public string Description => "Sets all players to the given role.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set all the players as.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            RoleTypeId roleType = (RoleTypeId)Arguments[1];

            foreach (Player player in (PlayerCollection)Arguments[0])
                player.Role.Set(roleType);

            return new(true);
        }
    }
}
