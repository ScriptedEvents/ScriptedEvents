namespace ScriptedEvents.Variables.Player.Misc
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Misc";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Player;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Staff(),
        };
    }

    public class Staff : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SERVERSTAFF}";

        /// <inheritdoc/>
        public string Description => "Gets all of the staff on the server (RA access)";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.RemoteAdminAccess);
    }
}
