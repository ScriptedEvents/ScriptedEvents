namespace ScriptedEvents.Actions
{
    using System;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    using Rule = ScriptedEvents.Structures.DamageRule;

    public class SpawnLoadoutRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOADOUTRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a spawn loadout rule, where you can modify which items are default for a certain class.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeId), "The role to set the default spawn inventory for.", true),
            new Argument("items", typeof(ItemType[]), "The ItemType to ", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            /*
            float multiplier = (float)Arguments[2];
            Rule rule = null;

            // Roles
            if (Parser.TryGetEnum((string)Arguments[0], out RoleTypeId attackerRole, script))
            {
                if (Parser.TryGetEnum((string)Arguments[1], out RoleTypeId receiverRole, script))
                {
                    rule = new(attackerRole, receiverRole);
                }
                else if (Parser.TryGetEnum((string)Arguments[1], out Team receiverTeam, script))
                {
                    rule = new(attackerRole, receiverTeam);
                }
                else if (Parser.TryGetPlayers(RawArguments[1], null, out PlayerCollection players, script))
                {
                    rule = new(attackerRole, players);
                }
            }
            else if (Parser.TryGetEnum((string)Arguments[1], out RoleTypeId attackerRole2, script))
            {
                if (Parser.TryGetEnum((string)Arguments[0], out Team receiverTeam2, script))
                {
                    rule = new(receiverTeam2, attackerRole2);
                }
                else if (Parser.TryGetPlayers(RawArguments[0], null, out PlayerCollection players2, script))
                {
                    rule = new(players2, attackerRole2);
                }
            }

            // Teams
            if (Parser.TryGetEnum((string)Arguments[0], out Team attackerTeam, script))
            {
                if (Parser.TryGetEnum((string)Arguments[1], out Team receiverTeam, script))
                {
                    rule = new(attackerTeam, receiverTeam);
                }
                else if (Parser.TryGetEnum((string)Arguments[1], out RoleTypeId receiverRole, script))
                {
                    rule = new(attackerTeam, receiverRole);
                }
                else if (Parser.TryGetPlayers(RawArguments[1], null, out PlayerCollection players, script))
                {
                    rule = new(attackerTeam, players);
                }
            }
            else if (Parser.TryGetEnum((string)Arguments[1], out Team attackerTeam2, script))
            {
                if (Parser.TryGetEnum((string)Arguments[0], out RoleTypeId receiverRole2, script))
                {
                    rule = new(receiverRole2, attackerTeam2);
                }
                else if (Parser.TryGetPlayers(RawArguments[0], null, out PlayerCollection players2, script))
                {
                    rule = new(players2, attackerTeam2);
                }
            }
            else if (Parser.TryGetPlayers(RawArguments[0], null, out PlayerCollection attackers, script) && Parser.TryGetPlayers(RawArguments[1], null, out PlayerCollection receivers, script))
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
            */
            return new(false, "not implemented");
        }
    }
}
