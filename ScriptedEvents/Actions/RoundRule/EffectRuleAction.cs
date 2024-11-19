using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class EffectRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "EffectRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Action for setting rules to apply effects to specific roles/teams/players on-spawn.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Set", "Rule to give effects."),
                new Option("Remove", "Removes a previously-established rule.")),
            new MultiTypeArgument(
                "target", 
                new[]
                {
                    typeof(RoleTypeId),
                    typeof(Team),
                    typeof(PlayerCollection),
                }, 
                "The players to affect, or the RoleType/Team/Players to give the effect.", 
                true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Defaults to 1. Used with 'Set' mode.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0]!.ToUpper();
            EffectType effect = (EffectType)Arguments[2]!;
            byte intensity = (byte?)Arguments[3] ?? 1;
            
            Effect eff = new(effect, 0, intensity);

            switch (mode, Arguments[1])
            {
                case ("SET", PlayerCollection players):
                    foreach (Player ply in players)
                    {
                        if (MainPlugin.EventHandlingModule.PermPlayerEffects.ContainsKey(ply))
                            MainPlugin.EventHandlingModule.PermPlayerEffects[ply].Add(eff);
                        else
                            MainPlugin.EventHandlingModule.PermPlayerEffects.Add(ply, new() { eff });
                    }
                    break;
                
                case ("SET", Team team):    
                    if (MainPlugin.EventHandlingModule.PermTeamEffects.ContainsKey(team))
                        MainPlugin.EventHandlingModule.PermTeamEffects[team].Add(eff);
                    else
                        MainPlugin.EventHandlingModule.PermTeamEffects.Add(team, new() { eff });
                    break;
                    
                case ("SET", RoleTypeId role):
                    if (MainPlugin.EventHandlingModule.PermRoleEffects.ContainsKey(role))
                        MainPlugin.EventHandlingModule.PermRoleEffects[role].Add(eff);
                    else
                        MainPlugin.EventHandlingModule.PermRoleEffects.Add(role, new() { eff });
                    break;
                
                case ("REMOVE", PlayerCollection players):
                    foreach (Player ply in players)
                    {
                        if (MainPlugin.EventHandlingModule.PermPlayerEffects.ContainsKey(ply))
                            MainPlugin.EventHandlingModule.PermPlayerEffects[ply].RemoveAll(e => e == eff);
                    }
                    break;
                
                case ("REMOVE", Team team):    
                    if (MainPlugin.EventHandlingModule.PermTeamEffects.ContainsKey(team))
                        MainPlugin.EventHandlingModule.PermTeamEffects[team].RemoveAll(e => e == eff);
                    break;
                    
                case ("REMOVE", RoleTypeId role):
                    if (MainPlugin.EventHandlingModule.PermRoleEffects.ContainsKey(role))
                        MainPlugin.EventHandlingModule.PermRoleEffects[role].RemoveAll(e => e == eff);
                    break;
                
                default:
                    throw new ImpossibleException();
            }
            return new(true);
        }
    }
}
