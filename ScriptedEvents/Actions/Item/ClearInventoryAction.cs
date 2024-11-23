namespace ScriptedEvents.Actions.Item
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class ClearInventoryAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ClearInventory";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Clears inventory of the specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Player player in (Player[])Arguments[0]!)
            {
                player.Inventory.UserInventory.ReserveAmmo.Clear();
                player.Inventory.SendAmmoNextFrame = true;
                player.ClearInventory();
            }

            return new(true);
        }
    }
}
