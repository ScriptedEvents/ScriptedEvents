namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class KillAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "KILL";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string Description => "Kills the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to kill.", true),
            new Argument("damageType", typeof(string), $"The {nameof(DamageType)} to apply, 'VAPORIZE' to vaporize the body, or a custom death message. Default: Unknown", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0];

            if (Arguments.Length > 1)
            {
                bool useDeathType = true;
                string customDeath = null;

                if (!VariableSystem.TryParse((string)Arguments[1], out DamageType damageType, script))
                {
                    useDeathType = false;
                    customDeath = Arguments.JoinMessage(1);
                }

                foreach (Player player in plys)
                {
                    if (customDeath is "VAPORIZE")
                        player.Vaporize();
                    else if (useDeathType)
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
