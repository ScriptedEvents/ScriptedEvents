namespace ScriptedEvents.Actions
{
    using System;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class InfectRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "INFECTRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

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
            RoleTypeId oldRole = (RoleTypeId)Arguments[0];
            RoleTypeId newRole = (RoleTypeId)Arguments[1];

            bool movePlayer = false;

            if (Arguments.Length > 2)
                movePlayer = (bool)Arguments[2];

            MainPlugin.Handlers.InfectionRules.RemoveAll(rule => rule.OldRole == oldRole);
            MainPlugin.Handlers.InfectionRules.Add(new(oldRole, newRole, movePlayer));

            return new(true);
        }
    }
}
