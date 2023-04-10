namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
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
            new Argument("spawnpoint", typeof(bool), "Use spawnpoint? default: true", false),
            new Argument("inventory", typeof(bool), "Use default inventory? default: true", false),
            new Argument("max", typeof(int), "The maximum amount of players to set the role of. Variables & Math are supported. (default: unlimited).", false),

        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!Enum.TryParse<RoleTypeId>(Arguments[1], true, out RoleTypeId roleType))
                return new(MessageType.InvalidRole, this, "role", Arguments[1]);

            bool setSpawnpoint = Arguments.Length == 2 || Arguments[2] is "TRUE" or "YES";
            bool setInventory = Arguments.Length <= 3 || Arguments[3] is "TRUE" or "YES";

            RoleSpawnFlags flags = RoleSpawnFlags.None;

            if (setSpawnpoint)
                flags |= RoleSpawnFlags.UseSpawnpoint;

            if (setInventory)
                flags |= RoleSpawnFlags.AssignInventory;

            int max = -1;

            if (Arguments.Length > 4)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(4)), script);

                if (!ConditionHelper.TryMath(formula, out MathResult result))
                {
                    return new(MessageType.NotANumberOrCondition, this, "max", formula, result);
                }

                if (result.Result < 0)
                {
                    return new(MessageType.LessThanZeroNumber, this, "max", result.Result);
                }

                max = Mathf.RoundToInt(result.Result);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out Player[] plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            foreach (Player player in plys)
                player.Role.Set(roleType, flags);

            return new(true);
        }
    }
}
