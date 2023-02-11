using Exiled.API.Features;
using Exiled.API.Features.Roles;
using MEC;
using PlayerRoles;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using Tesla = Exiled.API.Features.TeslaGate;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class TeslaAction : IScriptAction, IHelpInfo
    {
        public string Name => "TESLA";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Modifies tesla gates.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to run. Valid options: PLAYERS, ROLETYPE, DISABLE, ENABLE", true),
            new Argument("target", typeof(object), "The targets. Different type based on the mode.\nPLAYERS: A list of players.\nROLETYPE: A valid RoleType (eg. ClassD, Scp173, etc)\nDISABLE & ENABLE: None", true),
            new Argument("duration", typeof(float), "The time before reversing the affect.", false),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2)
            {
                if (Arguments.Length < 1 || Arguments[0] != "DISABLE")
                {
                    return new(false, "Missing arguments: Mode, Target, Duration (optional)");
                }
            }
            
            string mode = Arguments[0];
            string target = mode == "DISABLE" ? null : Arguments[1];
            string duration = Arguments.Length > 2 ? string.Join(string.Empty, Arguments.Skip(2)) : null;

            switch (mode)
            {
                case "PLAYERS":
                    if (!ScriptHelper.TryGetPlayers(target, null, out List<Player> players))
                        return new(false, "No players with the given parameters were found!");

                    foreach (Player player in players)
                    {
                        if (!Tesla.IgnoredPlayers.Contains(player))
                            Tesla.IgnoredPlayers.Add(player);
                    }

                    return Reverse(mode, players, duration);
                case "ROLETYPE":
                    if (!Enum.TryParse(target, out RoleTypeId roleType))
                        return new(false, "Invalid RoleType provided.");

                    if (!Tesla.IgnoredRoles.Contains(roleType))
                        Tesla.IgnoredRoles.Add(roleType);
                    return Reverse(mode, roleType, duration);
                case "DISABLE":
                    duration = Arguments.Length > 1 ? string.Join(string.Empty, Arguments.Skip(1)) : null;
                    MainPlugin.Handlers.TeslasDisabled = true;
                    return Reverse(mode, null, duration);
                case "ENABLE":
                    duration = Arguments.Length > 1 ? string.Join(string.Empty, Arguments.Skip(1)) : null;
                    MainPlugin.Handlers.TeslasDisabled = false;
                    return Reverse(mode, null, duration);
                default:
                    return new(false, $"Invalid mode '{mode}'. Valid options: PLAYERS, ROLETYPE, DISABLE, ENABLE");
            }
        }

        public ActionResponse Reverse(string mode, object target, string duration)
        {
            if (duration is null || string.IsNullOrWhiteSpace(duration))
                return new(true);

            float floatDuration;

            try
            {
                floatDuration = (float)ConditionHelper.Math(duration);
            }
            catch (Exception ex)
            {
                return new(false, $"Invalid duration condition provided! Condition: {duration} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
            }

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
