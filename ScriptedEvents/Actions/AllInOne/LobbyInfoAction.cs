namespace ScriptedEvents.Actions.AllInOne
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class LobbyInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "LobbyInfo";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting lobby related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new OptionValueDepending("WaitTime", "How many seconds are left until the round start.", typeof(short)),
                new OptionValueDepending("IsActive", "Is the lobby is active.", typeof(bool)),
                new OptionValueDepending("IsLocked", "Is the lobby is locked.", typeof(bool))),
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
                "WAITTIME" => Round.LobbyWaitingTime.ToString(),
                "ISACTIVE" => Round.IsLobby.ToUpper(),
                "ISLOCKED" => Round.IsLobbyLocked.ToUpper(),
                _ => throw new ArgumentException()
            };

            return new(true, new(ret));
        }
    }
}