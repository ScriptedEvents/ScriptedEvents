namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class RoundInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "GetRoundInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting round related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("IsLocked", "Is the roundlock on.", typeof(bool)),
                new OptionValueDepending("HasStarted", "Has the round started.", typeof(bool)),
                new OptionValueDepending("IsInProgress", "Is the round in progress.", typeof(bool)),
                new OptionValueDepending("HasEnded", "Has the round ended.", typeof(bool)),
                new OptionValueDepending("ElapsedRounds", "Amount of rounds that have progressed since the server has started.", typeof(int)),
                new OptionValueDepending("DurationSeconds", "Amount of seconds since the round started.", typeof(double))),
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
                "ISLOCKED" => Round.IsLocked.ToUpper(),
                "HASSTARTED" => Round.IsStarted.ToUpper(),
                "ISINPROGRESS" => Round.InProgress.ToUpper(),
                "HASENDED" => Round.IsEnded.ToUpper(),
                "ELAPSEDROUNDS" => Round.UptimeRounds.ToString(),
                "DURATIONSECONDS" => Round.ElapsedTime.TotalSeconds.ToString(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret));
        }
    }
}