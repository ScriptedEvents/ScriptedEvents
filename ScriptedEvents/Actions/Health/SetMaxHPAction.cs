namespace ScriptedEvents.Actions.Health
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class SetMaxHpAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SetMaxHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Sets maximum HP for specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("maxHealthPoints", typeof(float), "The amount of max health points to set", true, ArgFlag.BiggerThan0),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            foreach (Player ply in (Player[])Arguments[0]!)
                ply.MaxHealth = (float)Arguments[1]!;

            return new(true);
        }
    }
}
