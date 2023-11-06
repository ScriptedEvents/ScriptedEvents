namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class SetRoleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SETROLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets all players to the given role.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set all the players as.", true),
            new Argument("max", typeof(int), "The maximum amount of players to set the role of. Variables are supported. (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse<RoleTypeId>(Arguments[1], out RoleTypeId roleType, script))
                return new(MessageType.InvalidRole, this, "role", Arguments[1]);

            int max = -1;

            if (Arguments.Length > 2)
            {
                if (!VariableSystem.TryParse(Arguments[2], out max, script))
                    return new(MessageType.NotANumber, this, "max", Arguments[2]);
                if (max < 0)
                    return new(MessageType.LessThanZeroNumber, this, "max", max);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out PlayerCollection plys, script))
                return new(false, plys.Message);

            foreach (Player player in plys)
                player.Role.Set(roleType);

            return new(true);
        }
    }
}
