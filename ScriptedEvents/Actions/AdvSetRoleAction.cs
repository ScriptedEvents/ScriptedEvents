namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class AdvSetRoleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ADVSETROLE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets all players to the given role with advanced settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to set the role as.", true),
            new Argument("role", typeof(RoleTypeId), "The role to set all the players as.", true),
            new Argument("spawnpoint", typeof(bool), "Use spawnpoint? default: true", false),
            new Argument("inventory", typeof(bool), "Use default inventory? default: true", false),
            new Argument("max", typeof(int), "The maximum amount of players to set the role of. Variables are supported. (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse<RoleTypeId>(Arguments[1], out RoleTypeId roleType, script))
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
                if (!VariableSystem.TryParse(Arguments[4], out max, script))
                {
                    return new(MessageType.NotANumber, this, "max", Arguments[4]);
                }
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

            foreach (Player player in plys)
                player.Role.Set(roleType, flags);

            return new(true);
        }
    }
}