namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class RoundInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ROUNDINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting round related information.";

        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ISLOCKED", "Returns a TRUE/FALSE value being the roundlock status."),
                new("ISSTARTED", "Returns a TRUE/FALSE value saying if the round has started."),
                new("ISINPROGRESS", "Returns a TRUE/FALSE value saying if the round is in progress."),
                new("ISENDED", "Returns a TRUE/FALSE value saying if the round has ended."),
                new("UPTIMEROUNDS", "Returns the amount of rounds that have progressed since the server has started.")),
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
                "LOCKED" => Round.IsLocked.ToUpper(),
                "STARTED" => Round.IsStarted.ToUpper(),
                "INPROGRESS" => Round.InProgress.ToUpper(),
                "ENDED" => Round.IsEnded.ToUpper(),
                "UPTIMEROUNDS" => Round.UptimeRounds.ToString(),
                _ => throw new ArgumentException()
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}