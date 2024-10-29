using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    public class IntercomInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "IntercomInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting intercom related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                    new("Speaker", "Returns the player who is speaking on the intercom."),
                    new("IsReady", "Returns a TRUE/FALSE value saying if intercom ready to detonate."),
                    new("CooldownLeft", "Returns the amount of seconds of the cooldown remaining."),
                    new("TimeLeft", "Returns the amount of seconds left for speaking."),
                    new("InUse", "Returns a TRUE/FALSE value saying if intercom is in use already.")),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();

            if (mode == "SPEAKER")
            {
                return new(true, new(Intercom.Speaker));
            }

            string ret = mode switch
            {
                "ISREADY" => (!Intercom.InUse && Intercom.RemainingCooldown <= 0).ToUpper(),
                "INUSE" => Intercom.InUse.ToUpper(),
                "COOLDOWNLEFT" => Intercom.RemainingCooldown.ToString(),
                "TIMELEFT" => Intercom.SpeechRemainingTime.ToString(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret));
        }
    }
}