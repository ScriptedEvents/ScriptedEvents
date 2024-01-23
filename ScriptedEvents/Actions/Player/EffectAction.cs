namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

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

            if (!VariableSystem.TryParse<EffectType>(Arguments[2], out EffectType effect, script))
                return new(false, "Invalid effect type provided.");

            int intensity = 1;

            if (Arguments.Length > 3)
            {
                if (!VariableSystem.TryParse(Arguments[3], out intensity, script))
                    return new(MessageType.NotANumber, this, "intensity", Arguments[3]);

                if (intensity < 0)
                    return new(MessageType.LessThanZeroNumber, this, "intensity", Arguments[3]);
            }

            int duration = 0;

            if (Arguments.Length > 4)
            {
                if (!VariableSystem.TryParse(Arguments[4], out duration, script))
                    return new(MessageType.NotANumber, this, "duration", Arguments[4]);

                if (duration < 0)
                    return new(MessageType.LessThanZeroNumber, this, "duration", Arguments[4]);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[1], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

            switch (mode)
            {
                case "GIVE":
                    if (intensity > 255)
                        return new(false, "Effect intensity must be between 0-255.");

                    foreach (Player player in plys)
                    {
                        Effect eff = new(effect, duration, (byte)intensity);
                        player.SyncEffect(eff);
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
