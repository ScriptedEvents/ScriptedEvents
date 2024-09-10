namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class IntercomInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "INTERCOMINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting intercom related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                    new("SPEAKER", "Returns the player who is speaking on the intercom."),
                    new("ISREADY", "Returns a TRUE/FALSE value saying if intercom ready to detonate."),
                    new("COOLDOWNLEFT", "Returns the amount of seconds of the cooldown remaining."),
                    new("TIMELEFT", "Returns the amount of seconds left for speaking."),
                    new("INUSE", "Returns a TRUE/FALSE value saying if intercom is in use already.")),
        };

        public string[] RawArguments { get; set; }

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
                return new(true, variablesToRet: new[] { Intercom.Speaker is not null ? new[] { Intercom.Speaker } : Array.Empty<Player>() });
            }

            string ret = mode switch
            {
                "ISREADY" => (!Intercom.InUse && Intercom.RemainingCooldown <= 0).ToUpper(),
                "INUSE" => Intercom.InUse.ToUpper(),
                "COOLDOWNLEFT" => Intercom.RemainingCooldown.ToString(),
                "TIMELEFT" => Intercom.SpeechRemainingTime.ToString(),
                _ => throw new ArgumentException()
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}