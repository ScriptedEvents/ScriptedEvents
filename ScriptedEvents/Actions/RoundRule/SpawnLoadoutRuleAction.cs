using System;
using System.Linq;
using Exiled.Events.EventArgs.Player;
using PlayerRoles;
using PluginAPI.Roles;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class SpawnLoadoutRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LoadoutRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a spawn loadout rule, where you can modify which items are default for a certain ROLE.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true, 
                new Option("Set", "Sets a loadout rule, where 'items' argument needs to be provided."),
                new Option("Remove", "Removes a loadout rule for a given role. Argument 'items' does not need to be provided.")),
            new Argument("role", typeof(RoleTypeId), "The role to set the default spawn inventory for.", true),
            new Argument("items", typeof(ItemType[]), "The ItemTypes to add on spawn as the specified role.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var role = (RoleTypeId)Arguments[1]!;
            var items = Arguments[2] as ItemType[];
            
            if (string.Equals("set", (string)Arguments[0]!, StringComparison.OrdinalIgnoreCase))
            {
                if (items is null)
                {
                    var err = new ErrorInfo(
                        "Missing arguments",
                        "When using the 'Set' mode, items must be provided.",
                        Name
                    ).ToTrace();
                    return new(false, null, err);
                }
                
                EventHandlingModule.Singleton!.SpawnLoadoutRule[role] = items.ToList();
            }
            else
            {
                EventHandlingModule.Singleton!.SpawnLoadoutRule.Remove(role);
            }

            return new(true);
        }
    }
}
