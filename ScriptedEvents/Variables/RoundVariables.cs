namespace ScriptedEvents.Variables.Round
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
            new UptimeRound(),
            new RoundEnded(),
            new RoundInProgress(),
            new RoundStarted(),

            new RoundMinutes(),
            new RoundSeconds(),
            new LobbyTime(),
            new RoundLock(),
            new LobbyLock(),
        };
    }

    public class UptimeRound : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{UPTIMEROUND}";

        /// <inheritdoc/>
        public string Description => "The amount of rounds that have progressed since the server has started.";

        /// <inheritdoc/>
        public float Value => Round.UptimeRounds;
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

    public class RoundLock : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUNDLOCKED}";

        public string ReversedName => "{!ROUNDLOCKED}";

        /// <inheritdoc/>
        public string Description => "Returns the roundlock setting status.";

        /// <inheritdoc/>
        public bool Value
        {
            get
            {
                return Round.IsLocked;
            }
        }
    }

    public class LobbyLock : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{LOBBYLOCKED}";

        public string ReversedName => "{!LOBBYLOCKED}";

        /// <inheritdoc/>
        public string Description => "Returns the lobbylock setting status.";

        /// <inheritdoc/>
        public bool Value
        {
            get
            {
                return Round.IsLobbyLocked;
            }
        }
    }
}
