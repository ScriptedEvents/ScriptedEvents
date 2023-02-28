namespace ScriptedEvents.Actions
{
    using System;
    using PlayerRoles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class InfectRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "INFECTRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Creates a new infection rule.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("oldRole", typeof(RoleTypeId), "The role a player must die as to be infected.", true),
            new Argument("newRole", typeof(RoleTypeId), "The role a player will become.", true),
            new Argument("movePlayer", typeof(bool), "TRUE if the player should be moved to their death position, FALSE (or leave empty) to leave at spawn.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!Enum.TryParse(Arguments[0], true, out RoleTypeId oldRole))
                return new(MessageType.InvalidRole, this, "oldrole", Arguments[0]);

            if (!Enum.TryParse(Arguments[1], true, out RoleTypeId newRole))
                return new(MessageType.InvalidRole, this, "newrole", Arguments[1]);

            bool movePlayer = Arguments[2].ToUpper() is "TRUE" or "YES";

            MainPlugin.Handlers.InfectionRules.RemoveAll(rule => rule.OldRole == oldRole);
            MainPlugin.Handlers.InfectionRules.Add(new(oldRole, newRole, movePlayer));

            return new(true);
        }
    }
}
