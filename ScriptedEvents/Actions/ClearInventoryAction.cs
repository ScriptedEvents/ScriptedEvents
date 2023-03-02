namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class ClearInventoryAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEARINVENTORY";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Clears inventory of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to remove the items from.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            Player[] plys;

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out plys))
                return new(MessageType.NoPlayersFound, this, "players");

            foreach (Player player in plys)
            {
                player.ClearInventory();
            }

            return new(true);
        }
    }
}
