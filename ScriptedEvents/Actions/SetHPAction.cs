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

    public class SetHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "HP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Set the HP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("health", typeof(float), "The amount of health to set the player to. Variables are supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] plys, script))
                return new(MessageType.NoPlayersFound, this, "players");

            if (!VariableSystem.TryParse(Arguments[2], out float hp, script))
                return new(MessageType.NotANumber, this, "health", Arguments[2]);
            if (hp < 0)
                return new(MessageType.LessThanZeroNumber, this, "health", hp);

            foreach (Player ply in plys)
                ply.Health = hp;

            return new(true);
        }
    }
}
