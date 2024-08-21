namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EffectImmunityRuleAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "EFFECTIMMUNITYRULE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Action for setting rules to apply immunity to effects to specific roles/teams/players on-spawn.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Rule to give effect immunity."),
                new("REMOVE", "Removes a previously-established immunity.")),
            new Argument("target", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("effect", typeof(EffectType), "The effect give or remove immunity to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            string mode = Arguments[0].ToUpper();
            PlayerCollection players = (PlayerCollection)Arguments[1];
            EffectType effectType = (EffectType)Arguments[2];
            var dict = MainPlugin.GetModule<EventHandlingModule>().PlayerEffectImmunity;

            foreach (Player plr in players)
            {
                switch (mode)
                {
                    case "SET":
                        if (!dict.ContainsKey(plr))
                        {
                            dict[plr] = new List<EffectType>(new[] { effectType });
                            continue;
                        }

                        if (dict[plr].Contains(effectType))
                        {
                            continue;
                        }

                        dict[plr].Add(effectType);
                        continue;

                    case "REMOVE":
                        if (!dict.ContainsKey(plr))
                        {
                            continue;
                        }

                        if (!dict[plr].Contains(effectType))
                        {
                            continue;
                        }

                        dict[plr].Remove(effectType);
                        continue;
                }
            }

            return new(true);
        }
    }
}
