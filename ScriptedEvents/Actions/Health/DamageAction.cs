﻿namespace ScriptedEvents.Actions.Health
{
    using System;
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class DamageAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "Damage";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string Description => "Damages the specified players, allowing to also set a custom message for when players die as result of damage.";

        /// <inheritdoc/>
        public string LongDescription => $@"A base-game DamageType may be used to provide a base-game death message. Alternatively, a custom message may be used instead of a DamageType.
A full list of valid DamageType IDs (as of {DateTime.Now:g}) follows:
{string.Join("\n", ((DamageType[])Enum.GetValues(typeof(DamageType))).Select(r => $"- [{r:D}] {r}"))}";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The player(s) to damage.", true),
            new Argument("damage", typeof(float), "The amount of damage to apply.", true, ArgFlag.BiggerThan0),
            new Argument(
                "damageType",
                typeof(DamageType),
                $"The type of damage to apply. If a {nameof(DamageType)} is not matched, this will act as a custom message instead. Default: Unknown",
                false,
                ArgFlag.ParseToStringOnFail),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var plys = (Player[])Arguments[0]!;
            var damage = (float)Arguments[1]!;

            if (Arguments.Length < 3)
            {
                foreach (Player player in plys)
                    player.Hurt(damage);

                return new(true);
            }

            if (Arguments[2] is DamageType damageType)
            {
                foreach (Player player in plys)
                {
                    player.Hurt(damage, damageType);
                }

                return new(true);
            }

            foreach (Player player in plys)
            {
                player.Hurt(damage, Arguments[2]!.ToString());
            }

            return new(true);
        }
    }
}
