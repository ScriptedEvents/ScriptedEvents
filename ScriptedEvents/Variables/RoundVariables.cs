﻿namespace ScriptedEvents.Variables.Round
{
#pragma warning disable SA1402 // File may only contain a single type
    using Exiled.API.Features;
    using ScriptedEvents.Variables.Interfaces;

    public class RoundVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Round";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new RoundEnded(),
            new RoundInProgress(),
            new RoundStarted(),

            new RoundMinutes(),
            new RoundSeconds(),
            new LobbyTime(),
        };
    }

    public class RoundEnded : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDENDED}";

        /// <inheritdoc/>
        public string ReversedName => "{!ROUNDENDED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the round has ended.";

        /// <inheritdoc/>
        public bool Value => Round.IsEnded;
    }

    public class RoundInProgress : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDINPROGRESS}";

        /// <inheritdoc/>
        public string ReversedName => "{!ROUNDINPROGRESS}";

        /// <inheritdoc/>
        public string Description => "Whether or not the round is in progress.";

        /// <inheritdoc/>
        public bool Value => Round.InProgress;
    }

    public class RoundStarted : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDSTARTED}";

        /// <inheritdoc/>
        public string ReversedName => "{!ROUNDSTARTED}";

        /// <inheritdoc/>
        public string Description => "Whether or not the round has started.";

        /// <inheritdoc/>
        public bool Value => Round.IsStarted;
    }

    public class RoundMinutes : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDMINUTES}";

        /// <inheritdoc/>
        public string Description => "The amount of elapsed round time, in minutes.";

        /// <inheritdoc/>
        public float Value => (float)Round.ElapsedTime.TotalMinutes;
    }

    public class RoundSeconds : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDSECONDS}";

        /// <inheritdoc/>
        public string Description => "The amount of elapsed round time, in seconds.";

        /// <inheritdoc/>
        public float Value => (float)Round.ElapsedTime.TotalSeconds;
    }

    public class LobbyTime : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{LOBBYTIME}";

        /// <inheritdoc/>
        public string Description => "The amount of time remaining before the round starts. -1 if round already started.";

        /// <inheritdoc/>
        public float Value
        {
            get
            {
                if (Round.IsStarted)
                    return -1f;

                return Round.LobbyWaitingTime;
            }
        }
    }
}
