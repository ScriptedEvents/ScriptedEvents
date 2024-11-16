using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class AddEffectAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AddEffect";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Action for giving/removing player effects.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Default: 1.", true),
            new Argument("duration", typeof(float), "Duration of the effect.", false),
            new Argument(
                "addDurationIfActive", 
                typeof(bool), 
                "If TRUE and a player already has this effect, the duration will increase by the provided duration amount. If FALSE, the effect duration will be overwritten.",
                false
            ),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0]!;
            EffectType effect = (EffectType)Arguments[1]!;

            byte intensity = (byte?)Arguments[2] ?? 1;
            
            if (Arguments[4] is float duration)
            {
                duration = (float)Arguments[4];
                if (duration < 0)
                    return new(false);
            }

            if (intensity > 255)
                return new(false, "Effect intensity must be between 0-255.");

            foreach (Player player in plys)
            {
                Effect eff = new(effect, duration, (byte)intensity);
                player.SyncEffect(eff);
            }
                    

            return new(true);
        }
    }
}
