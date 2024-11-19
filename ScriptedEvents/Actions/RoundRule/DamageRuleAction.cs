using System;
using PlayerRoles;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class DamageRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DamageRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a new damage rule.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new MultiTypeArgument(
                "attackerRule", 
                new[]
                {
                    typeof(RoleTypeId),
                    typeof(Team),
                    typeof(PlayerCollection)
                }, 
                "The rule for the attacker (either a role, team, or a player collection)",
                true),
            new MultiTypeArgument(
                "receiverRule", 
                new[]
                {
                    typeof(RoleTypeId),
                    typeof(Team),
                    typeof(PlayerCollection)
                }, 
                "The rule for the receiver (either a role, team, or player variable)",
                true),
            new Argument("multiplier", typeof(float), "The damage multiplier to apply when an attacker attacks a receiver. Default is 100%.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            object attackerRuleValue = Arguments[0] switch
            {
                RoleTypeId role => role,
                Team team => team,
                PlayerCollection playerCollection => playerCollection,
                _ => throw new ImpossibleException()
            };

            object receiverRuleValue = Arguments[1] switch
            {
                RoleTypeId role => role,
                Team team => team,
                PlayerCollection playerCollection => playerCollection,
                _ => throw new ImpossibleException()
            };

            MainPlugin.EventHandlingModule.DamageRules.Add(
                new(attackerRuleValue, receiverRuleValue) { Multiplier = (float)Arguments[2]! }
            );

            return new(true);
        }
    }
}
