﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    using Rule = ScriptedEvents.Structures.DamageRule;

    public class DamageRule : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DAMAGERULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

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
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse(Arguments[2], out float multiplier, script))
                return new(MessageType.NotANumber, this, "multiplier", Arguments[2]);

            Rule rule = null;

            // Roles
            if (VariableSystem.TryParse(Arguments[0], out RoleTypeId attackerRole, script))
            {
                if (VariableSystem.TryParse(Arguments[1], out RoleTypeId receiverRole, script))
                {
                    rule = new(attackerRole, receiverRole);
                }
                else if (VariableSystem.TryParse(Arguments[1], out Team receiverTeam, script))
                {
                    rule = new(attackerRole, receiverTeam);
                }
                else if (VariableSystem.TryGetPlayers(Arguments[1], out IEnumerable<Player> players, script))
                {
                    rule = new(attackerRole, players);
                }
            }
            else if (VariableSystem.TryParse(Arguments[1], out RoleTypeId attackerRole2, script))
            {
                if (VariableSystem.TryParse(Arguments[0], out Team receiverTeam2, script))
                {
                    rule = new(receiverTeam2, attackerRole2);
                }
                else if (VariableSystem.TryGetPlayers(Arguments[0], out IEnumerable<Player> players2, script))
                {
                    rule = new(players2, attackerRole2);
                }
            }

            // Teams
            if (VariableSystem.TryParse(Arguments[0], out Team attackerTeam, script))
            {
                if (VariableSystem.TryParse(Arguments[1], out Team receiverTeam, script))
                {
                    rule = new(attackerTeam, receiverTeam);
                }
                else if (VariableSystem.TryParse(Arguments[1], out RoleTypeId receiverRole, script))
                {
                    rule = new(attackerTeam, receiverRole);
                }
                else if (VariableSystem.TryGetPlayers(Arguments[1], out IEnumerable<Player> players, script))
                {
                    rule = new(attackerTeam, players);
                }
            }
            else if (VariableSystem.TryParse(Arguments[1], out Team attackerTeam2, script))
            {
                if (VariableSystem.TryParse(Arguments[0], out RoleTypeId receiverRole2, script))
                {
                    rule = new(receiverRole2, attackerTeam2);
                }
                else if (VariableSystem.TryGetPlayers(Arguments[0], out IEnumerable<Player> players2, script))
                {
                    rule = new(players2, attackerTeam2);
                }
            }
            else if (VariableSystem.TryGetPlayers(Arguments[0], out IEnumerable<Player> attackers, script) && VariableSystem.TryGetPlayers(Arguments[1], out IEnumerable<Player> receivers, script))
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
