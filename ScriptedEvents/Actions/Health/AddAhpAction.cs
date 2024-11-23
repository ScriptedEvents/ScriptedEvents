namespace ScriptedEvents.Actions.Health
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class AddAhpAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AddAHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Modifies AHP of specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of artificial health to ADD to the player(s). Use a negative number to remove.", true),
            new Argument("limit", typeof(float), "The upper limit of AHP. Default: 75.", false, ArgFlag.BiggerOrEqual0),
            new Argument("decay", typeof(float), "The AHP decay rate (how much AHP is lost per second). Default: 1.2.", false, ArgFlag.BiggerOrEqual0),
            new Argument("efficacy", typeof(float), "The ratio of incoming damage absorbed by AHP. (1 = All; 0 = None). Default: 0.7.", false, ArgFlag.BiggerOrEqual0),
            new Argument("sustain", typeof(TimeSpan), "The time before the AHP begins to decay. Default: 0.", false),
            new Argument("persistent", typeof(bool), "Whether or not the AHP process is removed entirely when the AHP reaches 0. Default: FALSE.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var plys = (Player[])Arguments[0]!;
            float amount = (float)Arguments[1]!;
            float limit = (float?)Arguments[2] ?? 75f;
            float decay = (float?)Arguments[3] ?? 1.2f;
            float efficacy = (float?)Arguments[4] ?? 0.7f;
            float sustain = (float?)Arguments[5] ?? 0f;
            bool persistent = (bool?)Arguments[6] ?? false;

            foreach (Player ply in plys)
                ply.AddAhp(amount, limit, decay, efficacy, sustain, persistent);

            return new(true);
        }
    }
}
