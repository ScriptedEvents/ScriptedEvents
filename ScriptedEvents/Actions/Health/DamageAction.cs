namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DamageAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "DAMAGE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string Description => "Damages the targeted players.";

        /// <inheritdoc/>
        public string LongDescription => $@"A base-game DamageType may be used to provide a base-game death message. Alternatively, a custom message may be used instead of a DamageType.
A full list of valid DamageType IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((DamageType[])Enum.GetValues(typeof(DamageType))).Select(r => $"- [{r:d}] {r}"))}";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to damage.", true),
            new Argument("damage", typeof(float), "The amount of damage to apply.", true),
            new Argument("damageType", typeof(string), $"The type of damage to apply. If a {nameof(DamageType)} is not matched, this will act as a custom message instead. Default: Unknown", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            float damage = (float)Arguments[1];

            if (damage < 0)
                return new(MessageType.LessThanZeroNumber, this, "damage", damage);

            if (Arguments.Length > 2)
            {
                bool useDeathType = true;
                string customDeath = null;

                if (!SEParser.TryParse((string)Arguments[2], out DamageType damageType, script))
                {
                    useDeathType = false;
                    customDeath = Arguments.JoinMessage(2);
                }

                foreach (Player player in plys)
                {
                    if (useDeathType)
                        player.Hurt(damage, damageType);
                    else
                        player.Hurt(damage, string.IsNullOrWhiteSpace(customDeath) ? "Unknown" : customDeath);
                }

                return new(true);
            }

            foreach (Player player in plys)
                player.Hurt(damage, DamageType.Unknown);

            return new(true);
        }
    }
}
