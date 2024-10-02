namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class WarheadInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "WARHEADINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting warhead related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("IsDetonated", "Returns a TRUE/FALSE value saying if the warhead is detonated."),
                new("IsOpen", "Returns a TRUE/FALSE value saying if the warhead is open."),
                new("IsArmed", "Returns a TRUE/FALSE value saying if the warhead is armed."),
                new("IsCounting", "Returns a TRUE/FALSE value saying if the warhead is detonating."),
                new("TimeLeft", "Returns the amount of seconds remaining to the explosion.")),
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
            string ret = Arguments[0].ToUpper() switch
            {
                "ISDETONATED" => Warhead.IsDetonated.ToUpper(),
                "ISOPEN" => Warhead.IsKeycardActivated.ToUpper(),
                "ISARMED" => Warhead.LeverStatus.ToUpper(),
                "ISCOUNTING" => Warhead.IsInProgress.ToUpper(),
                "TIMELEFT" => Warhead.DetonationTimer.ToUpper(),
                _ => throw new ArgumentException("Invalid mode."),
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}