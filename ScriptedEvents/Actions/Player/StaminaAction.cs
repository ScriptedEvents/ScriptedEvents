namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class StaminaAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "STAMINA";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Modifies stamina of specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ADD", "Adds stamina for players."),
                new("SET", "Sets stamina for players."),
                new("REMOVE", "Removes stamina from players.")),
            new Argument("players", typeof(PlayerCollection), "Players to affect.", true),
            new Argument("amountPercent", typeof(float), "The amount of stamina percentage to add/set/remove.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];
            float amount = (float)Arguments[2] / 100;
            Action<Player, float> action = null;

            switch (Arguments[0].ToUpper())
            {
                case "ADD":
                    action = (player, amount) =>
                    {
                        player.Stamina += amount;
                    };
                    break;

                case "SET":
                    action = (player, amount) =>
                    {
                        player.Stamina = amount;
                    };
                    break;

                case "REMOVE":
                    action = (player, amount) =>
                    {
                        player.Stamina -= amount;
                    };
                    break;
            }

            foreach (Player player in players)
            {
                action(player, amount);
            }

            return new(true);
        }
    }
}