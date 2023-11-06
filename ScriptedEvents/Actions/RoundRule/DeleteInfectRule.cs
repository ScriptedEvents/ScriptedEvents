namespace ScriptedEvents.Actions
{
    using System;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class DeleteInfectRule : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DELINFECTRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Delete a currently-existing infection rule.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeId), "The role a player must die as to be infected.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse(Arguments[0], out RoleTypeId oldRole, script))
                return new(MessageType.InvalidRole, this, "role", Arguments[0]);

            MainPlugin.Handlers.InfectionRules.RemoveAll(rule => rule.OldRole == oldRole);

            return new(true);
        }
    }
}
