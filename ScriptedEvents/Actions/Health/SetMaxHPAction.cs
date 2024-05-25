namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class SetMaxHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "MAXHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Modifies maximum HP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("maxhealth", typeof(float), "The amount of max health to set", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            float hp = (float)Arguments[1];

            if (hp < 0)
                return new(MessageType.LessThanZeroNumber, this, "maxhealth", hp);

            foreach (Player ply in plys)
                ply.MaxHealth = hp;

            return new(true);
        }
    }
}
