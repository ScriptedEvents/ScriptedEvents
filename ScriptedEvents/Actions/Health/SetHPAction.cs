using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class SetHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Modifies HP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of health to set the player to.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(false, new ErrorTrace(new ErrorInfo("action not implemented", "action not implemented", "action not implemented")));
            /*
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            float hp = (float)Arguments[1];

            if (hp < 0)
                return new(MessageType.LessThanZeroNumber, this, "health", null, hp);

            foreach (Player ply in plys)
                ply.Health = hp;

            return new(true);
            */
        }
    }
}
