namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ClearInventoryAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CLEARINVENTORY";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Inventory;

        /// <inheritdoc/>
        public string Description => "Clears inventory of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to remove the items from.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            PlayerCollection plys = (PlayerCollection)Arguments[0];

            foreach (Player player in plys)
            {
                player.Inventory.UserInventory.ReserveAmmo.Clear();
                player.Inventory.SendAmmoNextFrame = true;
                player.ClearInventory();
            }

            return new(true);
        }
    }
}
