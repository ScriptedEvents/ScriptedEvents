namespace ScriptedEvents.Actions
{
    using System;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    using Rule = ScriptedEvents.Structures.DamageRule;

    public class DamageRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DAMAGERULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a new damage rule.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("attackerRule", typeof(object), "The rule for the attacker (either a role, team, or player variable)", true),
            new Argument("receiverRule", typeof(object), "The rule for the receiver (either a role, team, or player variable)", true),
            new Argument("multiplier", typeof(float), "The multiplier to apply to the damage rule.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float multiplier = (float)Arguments[2];
            Rule rule = null;

            // Roles
            if (VariableSystem.TryParse((string)Arguments[0], out RoleTypeId attackerRole, script))
            {
                if (VariableSystem.TryParse((string)Arguments[1], out RoleTypeId receiverRole, script))
                {
                    rule = new(attackerRole, receiverRole);
                }
                else if (VariableSystem.TryParse((string)Arguments[1], out Team receiverTeam, script))
                {
                    rule = new(attackerRole, receiverTeam);
                }
                else if (ScriptHelper.TryGetPlayers((string)Arguments[1], null, out PlayerCollection players, script))
                {
                    rule = new(attackerRole, players);
                }
            }
            else if (VariableSystem.TryParse((string)Arguments[1], out RoleTypeId attackerRole2, script))
            {
                if (VariableSystem.TryParse((string)Arguments[0], out Team receiverTeam2, script))
                {
                    rule = new(receiverTeam2, attackerRole2);
                }
                else if (ScriptHelper.TryGetPlayers((string)Arguments[0], null, out PlayerCollection players2, script))
                {
                    rule = new(players2, attackerRole2);
                }
            }

            // Teams
            if (VariableSystem.TryParse((string)Arguments[0], out Team attackerTeam, script))
            {
                if (VariableSystem.TryParse((string)Arguments[1], out Team receiverTeam, script))
                {
                    rule = new(attackerTeam, receiverTeam);
                }
                else if (VariableSystem.TryParse((string)Arguments[1], out RoleTypeId receiverRole, script))
                {
                    rule = new(attackerTeam, receiverRole);
                }
                else if (ScriptHelper.TryGetPlayers((string)Arguments[1], null, out PlayerCollection players, script))
                {
                    rule = new(attackerTeam, players);
                }
            }
            else if (VariableSystem.TryParse((string)Arguments[1], out Team attackerTeam2, script))
            {
                if (VariableSystem.TryParse((string)Arguments[0], out RoleTypeId receiverRole2, script))
                {
                    rule = new(receiverRole2, attackerTeam2);
                }
                else if (ScriptHelper.TryGetPlayers((string)Arguments[0], null, out PlayerCollection players2, script))
                {
                    rule = new(players2, attackerTeam2);
                }
            }
            else if (ScriptHelper.TryGetPlayers((string)Arguments[0], null, out PlayerCollection attackers, script) && ScriptHelper.TryGetPlayers((string)Arguments[1], null, out PlayerCollection receivers, script))
            {
                rule = new(attackers, receivers);
            }

            if (rule == null)
            {
                return new(false, "Invalid rule provided in the DAMAGERULE action.");
            }

            rule.Multiplier = multiplier;

            MainPlugin.Handlers.DamageRules.Add(rule);

            return new(true);
        }
    }
}
