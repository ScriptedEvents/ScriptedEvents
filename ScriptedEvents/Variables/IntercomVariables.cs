namespace ScriptedEvents.Variables.Intercom
{
    using System;
#pragma warning disable SA1402 // File may only contain a single type.
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Interfaces;

    public class IntercomVariables : IVariableGroup
    {
        public string GroupName => "Intercom";

        public IVariable[] Variables { get; } = new IVariable[]
        {
            new IntercomReady(),
            new IntercomInUse(),
            new IntercomCooldown(),
            new IntercomUseTime(),
            new IntercomSpeaker(),
        };
    }

    public class IntercomReady : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMREADY}";

        /// <inheritdoc/>
        public string ReversedName => "{!INTERCOMREADY}";

        /// <inheritdoc/>
        public string Description => "Indicates whether or not the intercom is ready to be used.";

        /// <inheritdoc/>
        public bool Value => !Intercom.InUse && Intercom.RemainingCooldown <= 0;
    }

    public class IntercomInUse : IBoolVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMINUSE}";

        /// <inheritdoc/>
        public string ReversedName => "{!INTERCOMINUSE}";

        /// <inheritdoc/>
        public string Description => "Indicates whether or not the intercom is currently being used.";

        /// <inheritdoc/>
        public bool Value => Intercom.InUse;
    }

    public class IntercomCooldown : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMCOOLDOWN}";

        /// <inheritdoc/>
        public string Description => "Indicates how much time is left on the intercom's cooldown.";

        /// <inheritdoc/>
        public float Value => (float)Intercom.RemainingCooldown;
    }

    public class IntercomUseTime : IFloatVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMUSETIME}";

        /// <inheritdoc/>
        public string Description => "Indicates how much time is left for the intercom speaker to speak.";

        /// <inheritdoc/>
        public float Value => Intercom.SpeechRemainingTime;
    }

    public class IntercomSpeaker : IFloatVariable, IPlayerVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOMSPEAKER}";

        /// <inheritdoc/>
        public string Description => "Gets the amount of players who are speaking on the intercom (always either 0 or 1).";

        /// <inheritdoc/>
        public float Value => Players.Count();

        /// <inheritdoc/>
        public IEnumerable<Player> Players => Player.Get(player => Intercom.Speaker == player);
    }

    public class GeneralIntercom : IStringVariable, IArgumentVariable
    {
        /// <inheritdoc/>
        public string Name => "{INTERCOM}";

        /// <inheritdoc/>
        public string Description => "All-in-one variable for Intercom related information.";

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (READY/INUSE/TIMELEFT/COOLDOWNTIME)", true),
        };

        /// <inheritdoc/>
        public string Value
        {
            get
            {
                string mode = (string)Arguments[0];

                return mode.ToUpper() switch
                {
                    "READY" => (!Intercom.InUse && Intercom.RemainingCooldown <= 0).ToString().ToUpper(),
                    "INUSE" => Intercom.InUse.ToString().ToUpper(),
                    "COOLDOWNTIME" => Intercom.RemainingCooldown.ToString(),
                    "TIMELEFT" => Intercom.SpeechRemainingTime.ToString(),
                    _ => throw new ArgumentException("Invalid mode.", mode),
                };
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
