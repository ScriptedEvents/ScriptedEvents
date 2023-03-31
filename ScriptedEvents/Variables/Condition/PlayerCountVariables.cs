namespace ScriptedEvents.Variables.Condition.PlayerCount
{
#pragma warning disable SA1402 // File may only contain a single type.
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class PlayerCountVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Players";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new Players(),
            new PlayersAlive(),
            new PlayersDead(),
        };
    }

    public class Players : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERS}";

        /// <inheritdoc/>
        public string Description => "The total amount of players in the server.";

        /// <inheritdoc/>
        public float Value => Player.List.Count();
    }

    public class PlayersAlive : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERSALIVE}";

        /// <inheritdoc/>
        public string Description => "The total amount of alive players in the server.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(p => p.IsAlive);
    }

    public class PlayersDead : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{PLAYERSDEAD}";

        /// <inheritdoc/>
        public string Description => "The total amount of dead players in the server.";

        /// <inheritdoc/>
        public float Value => Player.List.Count(p => p.IsDead);
    }
}
