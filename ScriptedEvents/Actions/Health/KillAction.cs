namespace ScriptedEvents.Actions.Health
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class KillAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "Kill";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string Description => "Kills the specified players.";

        /// <inheritdoc/>
        public string LongDescription => $@"A base-game DamageType may be used to provide a base-game death message. Alternatively, a custom message may be used instead of a DamageType.
A full list of valid DamageType IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((DamageType[])Enum.GetValues(typeof(DamageType))).Select(r => $"- [{r:d}] {r}"))}";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to kill.", true),
            new Argument(
                "damageType",
                typeof(DamageType),
                $"The {nameof(DamageType)} to apply, 'VAPORIZE' to vaporize the body, or a custom death message. Default: Unknown",
                false,
                ArgFlag.ParseToStringOnFail),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var plys = (Player[])Arguments[0]!;
            Action<Player> act = Arguments[1] switch
            {
                DamageType type => player => player.Kill(type),
                string reason => player => player.Kill(reason),
                null => player => player.Kill(DamageType.Unknown),
                _ => throw new ArgumentOutOfRangeException()
            };

            foreach (Player player in plys)
            {
                act(player);
            }

            return new(true);
        }
    }
}
