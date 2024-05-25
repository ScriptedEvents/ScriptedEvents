namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ReskinAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "RESKIN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets the appearance of all players to the given role. Does NOT actually change their role -- only their appearance!";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set the appearance of all the players as.", true),
            new Argument("targetPlayers", typeof(PlayerCollection), "The players that will see reskin taking place. Do not provide this variable for all players to see the reskin.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            RoleTypeId roleType = (RoleTypeId)Arguments[1];

            if (Arguments.Length >= 3)
            {
                PlayerCollection targetPlys = (PlayerCollection)Arguments[2];
                foreach (Player player in plys)
                    player.ChangeAppearance(roleType, targetPlys, false, 0);

                return new(true);
            }

            foreach (Player player in plys)
                player.ChangeAppearance(roleType, false, 0);

            return new(true);
        }
    }
}
