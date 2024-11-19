using System;
using PlayerRoles;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class SpawnRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SpawnRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a new spawn rule, modifying how players spawn at the start of the game. MUST BE USED BEFORE THE ROUND STARTS.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeId), "The role to create the rule for.", true),
            new Argument(
                "max", 
                typeof(int), 
                "The maximum amount of players to spawn as this role. If not provided, EVERY player who does not become a role with a different spawn rule will become this role.", 
                false, 
                ArgFlag.BiggerThan0
            ),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            RoleTypeId roleType = (RoleTypeId)Arguments[0]!;
            int max = (int?)Arguments[1] ?? -1;

            MainPlugin.EventHandlingModule.SpawnRules.Remove(roleType);
            MainPlugin.EventHandlingModule.SpawnRules.Add(roleType, max);

            return new(true);
        }
    }
}
