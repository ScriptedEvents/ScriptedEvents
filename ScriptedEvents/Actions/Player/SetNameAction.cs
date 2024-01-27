namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SetNameeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SETNAME";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets a player's name to the provided string.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to set the name for.", true),
            new Argument("name", typeof(RoleTypeId), "The name to set.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryGetPlayers(Arguments[0], out PlayerCollection players, script))
                return new(MessageType.NoPlayersFound, this, "players", Arguments[0]);

            string name = VariableSystem.ReplaceVariables(string.Join(" ", Arguments.Skip(1)));

            foreach (Player player in players)
            {
                player.DisplayNickname = name;
            }

            return new(true);
        }
    }
}