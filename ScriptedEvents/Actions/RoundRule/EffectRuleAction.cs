namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using PlayerRoles;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Modules;
    using ScriptedEvents.Structures;

    public class EffectRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "EFFECTRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Action for setting rules to apply effects to specific roles/teams/players on-spawn.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Rule to give effects."),
                new("REMOVE", "Removes a previously-established rule.")),
            new Argument("target", typeof(string), "The players to affect, or the RoleType/Team to give the effect.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Defaults to 1.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            EffectType effect = (EffectType)Arguments[2];

            int intensity = 1;
            if (Arguments.Length > 3)
            {
                intensity = (int)Arguments[3];
                if (intensity < 0 || intensity > 255)
                {
                    return new(false, "Intensity must be a whole number from 0-255.");
                }
            }

            int list = -1;

            Team team = Team.Dead;
            RoleTypeId rt = RoleTypeId.None;
            PlayerCollection players = null;

            if (SEParser.TryParse(RawArguments[1], out team, script))
            {
                list = 1;
            }
            else if (SEParser.TryParse(RawArguments[1], out rt, script))
            {
                list = 2;
            }
            else if (ScriptModule.TryGetPlayers(RawArguments[1], null, out players, script))
            {
                if (!players.Success)
                {
                    return new(false, players.Message);
                }

                list = 0;
            }

            if (list == -1)
            {
                return new(false, "Second argument (target) must be a Team, RoleType, or a player variable with at least one player.");
            }

            Effect eff = new(effect, 0, (byte)intensity, false, true);

            switch (mode)
            {
                case "SET":
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
            }

            return new(true);
        }
    }
}
