namespace ScriptedEvents.Actions.Health
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class HealAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Heal";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Heals specified players. Players will not be healed past their MaxHP.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of health to heal.", true, ArgFlag.BiggerThan0),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Player ply in (PlayerCollection)Arguments[0]!)
                ply.Heal((float)Arguments[1]!);

            return new(true);
        }
    }
}