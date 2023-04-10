﻿namespace ScriptedEvents.Variables.Player.Misc
{
#pragma warning disable SA1402 // File may only contain a single type
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
            new IntercomSpeaker(),
            new Staff(),
        };
    }

    public class IntercomSpeaker : IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMSPEAKER}";

        /// <inheritdoc/>
        public string Description => "Gets the player who is speaking on the intercom.";

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => Intercom.Speaker == player);
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
#pragma warning restore SA1402 // File may only contain a single type
}
