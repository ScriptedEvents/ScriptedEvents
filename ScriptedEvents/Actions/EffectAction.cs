﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;
    using UnityEngine;

    public class EffectAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "EFFECT";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Action for giving/removing player effects.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (GIVE, REMOVE)", true),
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Variables are supported. Defaults to 1.", false),
            new Argument("duration", typeof(int), "The duration of the effect, or no duration for a permanent effect. Variables are supported.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            string mode = Arguments[0].ToUpper();

            if (!Enum.TryParse<EffectType>(Arguments[2], true, out EffectType effect))
                return new(false, "Invalid effect type provided.");

            int duration = 0;

            if (Arguments.Length > 4)
            {
                if (!VariableSystem.TryParse(Arguments[4], out duration, script))
                    return new(MessageType.NotANumber, this, "duration", Arguments[4]);

                if (duration < 0)
                    return new(MessageType.LessThanZeroNumber, this, "duration", Arguments[4]);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[1], null, out Player[] plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            switch (mode)
            {
                case "GIVE":
                    string intensityString = "1";
                    if (Arguments.Length > 3)
                        intensityString = Arguments[3];

                    if (!VariableSystem.TryParse(intensityString, out int intensity))
                        return new(MessageType.NotANumber, this, "intensity", Arguments[3]);

                    if (intensity < 0 || intensity > 255)
                        return new(false, "Effect intensity must be between 0-255.");

                    foreach (Player player in plys)
                    {
                        player.ChangeEffectIntensity(effect, (byte)intensity);
                        player.EnableEffect(effect, duration);
                    }

                    return new(true);
                case "REMOVE":
                    foreach (Player player in plys)
                    {
                        player.DisableEffect(effect);
                    }

                    return new(true);
                default:
                    return new(MessageType.InvalidOption, this, "mode", mode, "GIVE/REMOVE");
            }
        }
    }
}
