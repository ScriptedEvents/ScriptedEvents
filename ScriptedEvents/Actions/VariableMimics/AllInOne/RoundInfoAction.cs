namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class RoundInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "ROUNDINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting round related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("IsLocked", "Returns the TRUE/FALSE value of the roundlock status."),
                new("HasStarted", "Returns the TRUE/FALSE value saying if the round has started."),
                new("IsInProgress", "Returns a TRUE/FALSE value saying if the round is in progress."),
                new("HasEnded", "Returns a TRUE/FALSE value saying if the round has ended."),
                new("ElapsedRounds", "Returns the amount of rounds that have progressed since the server has started."),
                new("Duration", "Returns the amount of seconds since the round started.")),
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
                "ISLOCKED" => Round.IsLocked.ToUpper(),
                "HASSTARTED" => Round.IsStarted.ToUpper(),
                "ISINPROGRESS" => Round.InProgress.ToUpper(),
                "HASENDED" => Round.IsEnded.ToUpper(),
                "ELAPSEDROUNDS" => Round.UptimeRounds.ToString(),
                "DURATION" => Round.ElapsedTime.TotalSeconds.ToString(),
                _ => throw new ArgumentException()
            };

            return new(true, variablesToRet: new[] { ret });
        }
    }
}