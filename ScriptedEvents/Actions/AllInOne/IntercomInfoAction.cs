namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class IntercomInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetIntercomInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting intercom related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                    new OptionValueDepending("Speaker", "Player who is speaking on the intercom. Empty if no player is speaking.", typeof(Player)),
                    new OptionValueDepending("IsReady", "Is the intercom is ready.", typeof(bool)),
                    new OptionValueDepending("CooldownLeft", "Amount of seconds of the cooldown remaining.", typeof(double)),
                    new OptionValueDepending("TimeLeft", "Amount of seconds left for speaking.", typeof(float)),
                    new OptionValueDepending("InUse", "Is intercom in use already.", typeof(bool))),
        };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.AllInOneInfo;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0]!.ToUpper();

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