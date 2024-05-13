namespace ScriptedEvents.Variables.Round
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type

    using Exiled.API.Features;

    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class RoundVariables : IVariableGroup
    {
        /// <inheritdoc/>
        public string GroupName => "Round";

        /// <inheritdoc/>
        public IVariable[] Variables { get; } = new IVariable[]
        {
            new UptimeRound(),
            new LobbyLock(),
            new GeneralRound(),
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

    public class GeneralRound : IBoolVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{ROUND}";

        public string ReversedName => "{!ROUND}";

        /// <inheritdoc/>
        public string Description => "All-in-one variable for round related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("LOCKED", "Retruns the roundlock status."),
                new("STARTED", "TRUE if round has started."),
                new("INPROGRESS", "TRUE if round is in progress, neither started or ended."),
                new("ENDED", "TRUE if round has ended.")),
        };

        /// <inheritdoc/>
        public bool Value
        {
            get
            {
                string mode = (string)Arguments[0];

                return mode.ToUpper() switch
                {
                    "LOCKED" => Round.IsLocked,
                    "STARTED" => Round.IsStarted,
                    "INPROGRESS" => Round.InProgress,
                    "ENDED" => Round.IsEnded,
                    _ => throw new ArgumentException("No mode provided")
                };
            }
        }

        public string[] RawArguments { get; set; }

        public object[] Arguments { get; set; }
    }
}
