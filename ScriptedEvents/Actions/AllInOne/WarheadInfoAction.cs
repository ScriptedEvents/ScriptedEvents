namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class WarheadInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "WarheadInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting warhead related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("HasDetonated", "Has the warhead detonated.", typeof(bool)),
                new OptionValueDepending("IsOpen", "Is the warhead control panel open.", typeof(bool)),
                new OptionValueDepending("IsArmed", "IS the warhead armed.", typeof(bool)),
                new OptionValueDepending("IsInProgress", "Is the warhead explosion in progress (counting to detonation).", typeof(bool)),
                new OptionValueDepending("TimeLeft", "Returns the amount of seconds remaining to the explosion.", typeof(float))),
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
            string ret = Arguments[0]!.ToUpper() switch
            {
                "ISDETONATED" => Warhead.IsDetonated.ToUpper(),
                "ISOPEN" => Warhead.IsKeycardActivated.ToUpper(),
                "ISARMED" => Warhead.LeverStatus.ToUpper(),
                "ISCOUNTING" => Warhead.IsInProgress.ToUpper(),
                "TIMELEFT" => Warhead.DetonationTimer.ToUpper(),
                _ => throw new ArgumentException("Invalid mode."),
            };

            return new(true, new(ret));
        }
    }
}