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
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("effect", typeof(EffectType), "The effect to give or remove.", true),
            new Argument("intensity", typeof(byte), "The intensity of the effect, between 0-255. Default: 1.", true),
            new Argument("duration", typeof(float), "Duration of the effect. Default: Infinity", false),
            new Argument(
                "addDurationIfActive", 
                typeof(bool), 
                "If TRUE and a player already has this effect, the duration will increase by the provided duration amount. If FALSE, the effect duration will be overwritten. Default: FALSE.",
                false
            ),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] plys = (Player[])Arguments[0]!;
            EffectType effect = (EffectType)Arguments[1]!;

            byte intensity = (byte?)Arguments[2] ?? 1;
            float duration = (float?)Arguments[3] ?? float.PositiveInfinity;
            bool addDurationIfActive = (bool?)Arguments[4] ?? false;

            foreach (Player player in plys)
            {
                player.SyncEffect(new(effect, duration, intensity, addDurationIfActive));
            }

            return new(true);
        }
    }
}
