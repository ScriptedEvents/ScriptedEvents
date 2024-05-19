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
            new IntercomSpeaker(),
            new GeneralIntercom(),
        };
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
            new OptionsArgument("mode", true,
                    new("READY", "Is ready to detonate."),
                    new("COOLDOWN", "The cooldown remaining."),
                    new("TIMELEFT", "The time left for speaking."),
                    new("INUSE", "Is in use already.")),
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
                    "COOLDOWN" => Intercom.RemainingCooldown.ToString(),
                    "TIMELEFT" => Intercom.SpeechRemainingTime.ToString(),
                    _ => throw new ArgumentException("Invalid mode.", mode),
                };
            }
        }
    }
#pragma warning restore SA1402 // File may only contain a single type.
}
