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

    public class SetAHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Add AHP to the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of artificial health to add to the player. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

            if (!VariableSystem.TryParse(Arguments[1], out float hp, script))
                return new(MessageType.NotANumber, this, "health", Arguments[1]);
            if (hp < 0)
                return new(MessageType.LessThanZeroNumber, this, "health", hp);

            foreach (Player ply in plys)
                ply.AddAhp(hp);

            return new(true);
        }
    }
}
