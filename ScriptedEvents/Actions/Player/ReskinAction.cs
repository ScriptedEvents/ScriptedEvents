namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class ReskinAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "RESKIN";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets the appearance of all players to the given role. Does NOT actually change their role -- only their appearance!";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set the appearance of all the players as.", true),
            new Argument("targetPlayers", typeof(List<Player>), "The players that will see reskin taking place. Do not provide this variable for all players to see the reskin.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse<RoleTypeId>(Arguments[1], out RoleTypeId roleType, script))
                return new(MessageType.InvalidRole, this, "role", Arguments[1]);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

            if (Arguments.Length >= 3)
            {
                if (!ScriptHelper.TryGetPlayers(Arguments[2], null, out PlayerCollection tagretPlys, script))
                    return new(false, plys.Message);

                foreach (Player player in plys)
                    player.ChangeAppearance(roleType, tagretPlys, false, 0);

                return new(true);
            }

            foreach (Player player in plys)
                player.ChangeAppearance(roleType, false, 0);

            return new(true);
        }
    }
}
