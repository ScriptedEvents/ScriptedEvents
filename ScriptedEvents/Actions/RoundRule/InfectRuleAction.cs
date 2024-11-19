using System;
using PlayerRoles;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class InfectRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "InfectRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a new infection rule.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("oldRole", typeof(RoleTypeId), "The role a player must die as to be infected.", true),
            new Argument("newRole", typeof(RoleTypeId), "The role a player will become.", true),
            new Argument("movePlayer", typeof(bool), "TRUE if the player should be moved to the attacker on death, FALSE (or leave empty) to leave at spawn.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            RoleTypeId oldRole = (RoleTypeId)Arguments[0]!;
            RoleTypeId newRole = (RoleTypeId)Arguments[1]!;
            bool movePlayer = (bool?)Arguments[2] ?? false;

            EventHandlingModule.Singleton!.InfectionRules.RemoveAll(rule => rule.OldRole == oldRole);
            EventHandlingModule.Singleton!.InfectionRules.Add(new(oldRole, newRole, movePlayer));

            return new(true);
        }
    }
}
