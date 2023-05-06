namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using PlayerRoles;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables.Handlers;
    using UnityEngine;

    public class EffectPermanentAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "EFFECTPERM";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Action for giving/removing permanent player effects.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (GIVE, REMOVE)", true),
            new Argument("target", typeof(object), "The players to affect, or the RoleType/Team to infect with the role.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Math and variables are NOT supported. Defaults to 1.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string mode = Arguments[0].ToUpper();

            if (!Enum.TryParse<EffectType>(Arguments[2], true, out EffectType effect))
                return new(false, "Invalid effect type provided.");

            byte intensity = 1;
            if (Arguments.Length > 3)
            {
                if (!byte.TryParse(Arguments[3], out intensity))
                {
                    return new(false, "Intensity must be a whole number from 1-255.");
                }
            }

            int list = -1;

            Team team = Team.Dead;
            RoleTypeId rt = RoleTypeId.None;
            Player[] players = null;

            if (Enum.TryParse(Arguments[1], true, out team))
            {
                list = 1;
            }
            else if (Enum.TryParse(Arguments[1], true, out rt))
            {
                list = 2;
            }
            else if (ScriptHelper.TryGetPlayers(Arguments[1], null, out players, script))
            {
                list = 0;
            }

            if (list == -1)
            {
                return new(false, "Second argument (target) must be a Team, RoleType, or a player variable with at least one player.");
            }

            Effect eff = new(effect, 0, intensity, false, true);

            switch (mode)
            {
                case "GIVE":
                    if (list is 0)
                    {
                        foreach (Player ply in players)
                        {
                            if (MainPlugin.Handlers.PermPlayerEffects.ContainsKey(ply))
                                MainPlugin.Handlers.PermPlayerEffects[ply].Add(eff);
                            else
                                MainPlugin.Handlers.PermPlayerEffects.Add(ply, new() { eff });
                        }
                    }
                    else if (list is 1)
                    {
                        if (MainPlugin.Handlers.PermTeamEffects.ContainsKey(team))
                            MainPlugin.Handlers.PermTeamEffects[team].Add(eff);
                        else
                            MainPlugin.Handlers.PermTeamEffects.Add(team, new() { eff });
                    }
                    else if (list is 2)
                    {
                        if (MainPlugin.Handlers.PermRoleEffects.ContainsKey(rt))
                            MainPlugin.Handlers.PermRoleEffects[rt].Add(eff);
                        else
                            MainPlugin.Handlers.PermRoleEffects.Add(rt, new() { eff });
                    }

                    break;
                case "REMOVE":
                    if (list is 0)
                    {
                        foreach (Player ply in players)
                        {
                            if (MainPlugin.Handlers.PermPlayerEffects.ContainsKey(ply))
                                MainPlugin.Handlers.PermPlayerEffects[ply].RemoveAll(e => e == eff);
                        }
                    }
                    else if (list is 1)
                    {
                        if (MainPlugin.Handlers.PermTeamEffects.ContainsKey(team))
                            MainPlugin.Handlers.PermTeamEffects[team].RemoveAll(e => e == eff);
                    }
                    else if (list is 2)
                    {
                        if (MainPlugin.Handlers.PermRoleEffects.ContainsKey(rt))
                            MainPlugin.Handlers.PermRoleEffects[rt].RemoveAll(e => e == eff);
                    }

                    break;
                default:
                    return new(MessageType.InvalidOption, this, "mode", mode, "GIVE/REMOVE");
            }

            return new(true);
        }
    }
}
