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

    public class KillAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "KILL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string Description => "Kills the targeted players.";

        /// <inheritdoc/>
        public string LongDescription => $@"A base-game DamageType may be used to provide a base-game death message. Alternatively, a custom message may be used instead of a DamageType.
A full list of valid DamageType IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((DamageType[])Enum.GetValues(typeof(DamageType))).Select(r => $"- [{r:d}] {r}"))}";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to kill.", true),
            new Argument("damageType", typeof(string), $"The {nameof(DamageType)} to apply, 'VAPORIZE' to vaporize the body, or a custom death message. Default: Unknown", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];

            if (Arguments.Length > 1)
            {
                bool useDeathType = true;
                string customDeath = null;

                if (!SEParser.TryParse((string)Arguments[1], out DamageType damageType, script))
                {
                    useDeathType = false;
                    customDeath = Arguments.JoinMessage(1);
                }

                foreach (Player player in plys)
                {
                    if (customDeath is "VAPORIZE")
                        player.Vaporize();
                    else if (useDeathType)
                        player.Kill(damageType);
                    else
                        player.Kill(string.IsNullOrWhiteSpace(customDeath) ? "Unknown" : customDeath);
                }

                return new(true);
            }

            foreach (Player player in plys) player.Kill(DamageType.Unknown);

            return new(true);
        }
    }
}
