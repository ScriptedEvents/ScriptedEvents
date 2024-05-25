namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class SetAHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Modifies AHP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of artificial health to ADD to the player. Use a negative number to remove.", true),
            new Argument("limit", typeof(float), "The upper limit of AHP. Default: 75.", false),
            new Argument("decay", typeof(float), "The AHP decay rate (how much AHP is lost per second). Default: 1.2.", false),
            new Argument("efficacy", typeof(float), "The percent of incoming damage absorbed by AHP. Default: 0.7.", false),
            new Argument("sustain", typeof(float), "The amount of time (in seconds) before the AHP begins to decay. Default: 0.", false),
            new Argument("persistent", typeof(bool), "Whether or not the AHP process is removed entirely when the AHP reaches 0. Default: FALSE.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];
            float hp = (float)Arguments[1];

            float limit = 75;
            if (Arguments.Length > 2)
            {
                limit = (float)Arguments[2];
                if (limit < 0)
                    return new(MessageType.LessThanZeroNumber, this, "limit", limit);
            }

            float decay = 1.2f;
            if (Arguments.Length > 3)
            {
                decay = (float)Arguments[3];
                if (decay < 0)
                    return new(MessageType.LessThanZeroNumber, this, "decay", limit);
            }

            float efficacy = 0.7f;
            if (Arguments.Length > 4)
            {
                efficacy = (float)Arguments[4];
                if (efficacy < 0)
                    return new(MessageType.LessThanZeroNumber, this, "efficacy", limit);
            }

            float sustain = 0f;
            if (Arguments.Length > 5)
            {
                sustain = (float)Arguments[5];
                if (sustain < 0)
                    return new(MessageType.LessThanZeroNumber, this, "sustain", limit);
            }

            bool persistent = Arguments.Length > 6 && (bool)Arguments[6];

            foreach (Player ply in plys)
                ply.AddAhp(hp, limit, decay, efficacy, sustain, persistent);

            return new(true);
        }
    }
}
