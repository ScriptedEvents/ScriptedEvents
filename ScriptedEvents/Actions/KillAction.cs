namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class KillAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "KILL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Kills the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to kill.", true),
            new Argument("damageType", typeof(string), "The DeathType to apply. Default: Unknown", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] plys))
                return new(MessageType.NoPlayersFound, this, "players");

            if (Arguments.Length > 1)
            {
                if (!Enum.TryParse(Arguments[1], out DamageType damageType))
                    return new(false, "Invalid DamageType provided.");

                foreach (Player player in plys)
                    player.Kill(damageType);

                return new(true);
            }

            foreach (Player player in plys)
                player.Kill(DamageType.Unknown);

            return new(true);
        }
    }
}
