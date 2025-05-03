using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Item
{
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
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Clears inventory of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];

            foreach (Exiled.API.Features.Player player in plys)
            {
                player.Inventory.UserInventory.ReserveAmmo.Clear();
                player.Inventory.SendAmmoNextFrame = true;
                player.ClearInventory();
            }

            return new(true);
        }
    }
}
