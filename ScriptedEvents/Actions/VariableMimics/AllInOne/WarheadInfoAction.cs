namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class WarheadInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "WARHEADINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting warhead related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ISDETONATED", "Returns a TRUE/FALSE value saying if the warhead is detonated."),
                new("ISOPEN", "Returns a TRUE/FALSE value saying if the warhead is open."),
                new("ISARMED", "Returns a TRUE/FALSE value saying if the warhead is armed."),
                new("ISCOUNTING", "Returns a TRUE/FALSE value saying if the warhead is detonating."),
                new("DETONATIONTIME", "Returns the amount of seconds remaining to the explosion.")),
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
            string ret = Arguments[0].ToUpper() switch
            {
                "ISDETONATED" => Warhead.IsDetonated.ToUpper(),
                "ISOPEN" => Warhead.IsKeycardActivated.ToUpper(),
                "ISARMED" => Warhead.LeverStatus.ToUpper(),
                "ISCOUNTING" => Warhead.IsInProgress.ToUpper(),
                "DETONATIONTIME" => Warhead.DetonationTimer.ToUpper(),
                _ => throw new ArgumentException("Invalid mode."),
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}