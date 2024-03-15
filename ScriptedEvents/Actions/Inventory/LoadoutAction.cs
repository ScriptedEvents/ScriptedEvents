namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class LoadoutAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOADOUT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Inventory;

        /// <inheritdoc/>
        public string Description => "Gives players a class loadout.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to grant loadout to.", true),
            new Argument("class", typeof(RoleTypeId), "The class of which the loadout will be granted.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            RoleTypeId role = (RoleTypeId)Arguments[1];

            foreach (Player player in plys)
            {
                foreach (ItemType itemType in role.GetStartingInventory())
                {
                    player.AddItem(itemType);
                }
            }

            return new(true);
        }
    }
}
