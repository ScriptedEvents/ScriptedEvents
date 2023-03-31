namespace ScriptedEvents.Variables.Condition.RoundTime
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using GameCore;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Variables.Interfaces;

    public class RoundTimeVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Round Time";

        /// <inheritdoc/>
        public VariableGroupType GroupType => VariableGroupType.Condition;

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new RoundMinutes(),
            new RoundSeconds(),
            new LobbyWaitingTime(),
        };
    }

    public class RoundMinutes : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDMINUTES}";

        /// <inheritdoc/>
        public string Description => "The total amount of elapsed round time, in minutes.";

        /// <inheritdoc/>
        public float Value => (float)Round.ElapsedTime.TotalMinutes;
    }

    public class RoundSeconds : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDSECONDS}";

        /// <inheritdoc/>
        public string Description => "The total amount of elapsed round time, in seconds.";

        /// <inheritdoc/>
        public float Value => (float)Round.ElapsedTime.TotalSeconds;
    }

    public class LobbyWaitingTime : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{LOBBYWAITINGTIME}";

        /// <inheritdoc/>
        public string Description => "The total ammount of time to wait.";

        /// <inheritdoc/>
        public float Value => RoundStart.singleton.NetworkTimer;
    }
}
