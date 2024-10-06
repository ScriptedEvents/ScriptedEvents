namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    /// <inheritdoc/>
    public class LobbyInfoAction : IScriptAction, IHelpInfo, IMimicsVariableAction
    {
        /// <inheritdoc/>
        public string Name => "LOBBYINFO";

        /// <inheritdoc/>
        public string Description => "All-in-one action for getting lobby related information.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("WaitTime", "Returns a number saying how many seconds are left until the round start."),
                new("IsActive", "Returns a TRUE/FALSE value saying if the lobby is active."),
                new("IsLocked", "Returns a TRUE/FALSE value saying if the lobby is locked.")),
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

            return new(true, variablesToRet: new[] { ret });
        }
    }
}