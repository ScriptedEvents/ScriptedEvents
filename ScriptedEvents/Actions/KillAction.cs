namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
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
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Kills the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to kill.", true),
            new Argument("damageType", typeof(DamageType), "The DeathType to apply. Default: Unknown", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script scr)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out Player[] plys))
                return new(MessageType.NoPlayersFound, this, "players");

            if (Arguments.Length > 1)
            {
                bool useDeathType = true;
                string customDeath = null;

                if (!Enum.TryParse(Arguments[2], true, out DamageType damageType))
                {
                    useDeathType = false;
                    customDeath = string.Join(" ", Arguments.Skip(1));
                }

                foreach (Player player in plys)
                {
                    if (useDeathType)
                        player.Kill(damageType);
                    else
                        player.Kill(string.IsNullOrWhiteSpace(customDeath) ? "Unknown" : customDeath);
                }

                return new(true);
            }

            foreach (Player player in plys)
                player.Kill(DamageType.Unknown);

            return new(true);
        }
    }
}
