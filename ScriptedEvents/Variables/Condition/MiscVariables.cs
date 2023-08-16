namespace ScriptedEvents.Variables.Condition.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class MiscVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Misc";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new IntercomSpeaker(),
            new Staff(),
        };
    }

    public class IntercomSpeaker : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMSPEAKER}";

        /// <inheritdoc/>
        public string Description => "Gets the player who is speaking on the intercom.";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => Intercom.Speaker == player);
    }

    public class Staff : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{SERVERSTAFF}";

        /// <inheritdoc/>
        public string Description => "Gets all of the staff on the server (RA access)";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => player.RemoteAdminAccess);
    }
#pragma warning restore SA1402 // File may only contain a single type
}
