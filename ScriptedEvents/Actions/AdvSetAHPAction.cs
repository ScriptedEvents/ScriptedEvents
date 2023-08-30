namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class AdvSetAHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ADVAHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Add AHP to the targeted players, with advanced settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of artificial health to add to the player.", true),
            new Argument("limit", typeof(float), "The upper limit of AHP. Defaults to 75.", false),
            new Argument("decay", typeof(float), "The AHP decay rate (how much AHP is lost per second). Defaults to 1.2.", false),
            new Argument("efficacy", typeof(float), "The percent of incoming damage absorbed by AHP. Defaults to 0.7 (70%).", false),
            new Argument("sustain", typeof(float), "The amount of time (in seconds) before the AHP begins to decay. Defaults to 0.", false),
            new Argument("persistent", typeof(bool), "Whether or not the AHP process is removed entirely when the AHP reaches 0. Defaults to FALSE.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            // Todo: Needs negative number checking
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            if (!VariableSystem.TryParse(Arguments[1], out float hp, script))
                return new(MessageType.NotANumber, this, "health", Arguments[1]);

            float limit = 75;
            if (Arguments.Length > 2)
            {
                if (!VariableSystem.TryParse(Arguments[2], out limit, script))
                    return new(MessageType.NotANumber, this, "limit", Arguments[2]);
            }

            float decay = 1.2f;
            if (Arguments.Length > 3)
            {
                if (!VariableSystem.TryParse(Arguments[3], out decay, script))
                    return new(MessageType.NotANumber, this, "decay", Arguments[2]);
            }

            float efficacy = 0.7f;
            if (Arguments.Length > 4)
            {
                if (!VariableSystem.TryParse(Arguments[4], out efficacy, script))
                    return new(MessageType.NotANumber, this, "efficacy", Arguments[2]);
            }

            float sustain = 0f;
            if (Arguments.Length > 5)
            {
                if (!VariableSystem.TryParse(Arguments[5], out sustain, script))
                    return new(MessageType.NotANumber, this, "sustain", Arguments[5]);
            }

            bool persistent = Arguments.Length > 6 && (Arguments[6] is "TRUE" or "YES");

            foreach (Player ply in plys)
                ply.AddAhp(hp, limit, decay, efficacy, sustain, persistent);

            return new(true);
        }
    }
}
