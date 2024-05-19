namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using MEC;

    using PlayerRoles;

    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    using Tesla = Exiled.API.Features.TeslaGate;

    // Todo: Needs reworked entirely
    public class TeslaAction : IScriptAction, IHelpInfo, ISampleAction
    {
        /// <inheritdoc/>
        public string Name => "TESLA";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies tesla gates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("PLAYERS", "Disables tesla gates for specific players."),
                new("ROLETYPE", "Disables tesla gates for a specific role."),
                new("DISABLE", "Disables tesla gates entirely."),
                new("ENABLE", "Enables tesla gates after previously disabling them.")),
            new Argument("target", typeof(object), "The targets. Different type based on the mode.\nPLAYERS: A list of players.\nROLETYPE: A valid RoleType (eg. ClassD, Scp173, etc)\nDISABLE & ENABLE: None", false),
            new Argument("duration", typeof(float), "The time before reversing the effect.", false),
        };

        public ISampleProvider Samples { get; } = new TeslaSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2)
            {
                if (Arguments.Length < 1 || (string)Arguments[0] != "DISABLE")
                {
                    return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);
                }
            }

            string mode = Arguments[0].ToUpper();
            string target = mode == "DISABLE" ? null : RawArguments[1];
            string duration = Arguments.Length > 2 ? Arguments.JoinMessage(2, string.Empty) : null;

            switch (mode)
            {
                case "PLAYERS":
                    if (!ScriptModule.TryGetPlayers(target, null, out PlayerCollection players, script))
                        return new(false, players.Message);

                    foreach (Player player in players)
                    {
                        if (!Tesla.IgnoredPlayers.Contains(player))
                            Tesla.IgnoredPlayers.Add(player);
                    }

                    return Reverse(mode, players, duration, script);
                case "ROLETYPE":
                    if (!SEParser.TryParse(target, out RoleTypeId roleType, script))
                        return new(MessageType.InvalidRole, this, "target", target);

                    if (!Tesla.IgnoredRoles.Contains(roleType))
                        Tesla.IgnoredRoles.Add(roleType);
                    return Reverse(mode, roleType, duration, script);
                case "DISABLE":
                    duration = Arguments.Length > 1 ? Arguments.JoinMessage(1, string.Empty) : null;
                    MainPlugin.Handlers.TeslasDisabled = true;
                    return Reverse(mode, null, duration, script);
                case "ENABLE":
                    duration = Arguments.Length > 1 ? Arguments.JoinMessage(1, string.Empty) : null;
                    MainPlugin.Handlers.TeslasDisabled = false;
                    return Reverse(mode, null, duration, script);
            }

            return new(true);
        }

        /// <summary>
        /// Reverses a previous action.
        /// </summary>
        /// <param name="mode">The mode to reverse.</param>
        /// <param name="target">The targeted objects.</param>
        /// <param name="duration">The duration until reversing.</param>
        /// <param name="script">The script.</param>
        /// <returns>An <see cref="ActionResponse"/> indicating success of the reverse.</returns>
        public ActionResponse Reverse(string mode, object target, string duration, Script script = null)
        {
            if (duration is null || string.IsNullOrWhiteSpace(duration))
                return new(true);

            if (!SEParser.TryParse(duration, out float floatDuration, script))
                return new(MessageType.NotANumber, this, "duration", duration);
            if (floatDuration < 0)
                return new(MessageType.LessThanZeroNumber, this, "duration", duration);

            Timing.CallDelayed(floatDuration, () =>
            {
                switch (mode)
                {
                    case "PLAYERS":
                        foreach (Player player in (List<Player>)target)
                        {
                            if (Tesla.IgnoredPlayers.Contains(player))
                                Tesla.IgnoredPlayers.Remove(player);
                        }

                        break;
                    case "ROLETYPE":
                        RoleTypeId roleType = (RoleTypeId)target;
                        if (Tesla.IgnoredRoles.Contains(roleType))
                            Tesla.IgnoredRoles.Remove(roleType);
                        break;
                    case "DISABLE":
                        MainPlugin.Handlers.TeslasDisabled = false;
                        break;
                    case "ENABLE":
                        MainPlugin.Handlers.TeslasDisabled = true;
                        break;
                }
            });

            return new(true);
        }
    }
}
