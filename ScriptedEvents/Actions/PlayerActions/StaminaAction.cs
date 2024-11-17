using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class StaminaAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Stamina";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Modifies stamina of specified players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Add", "Adds stamina for players."),
                new Option("Set", "Sets stamina for players."),
                new Option("Remove", "Removes stamina from players.")),
            new Argument("players", typeof(PlayerCollection), "Players to affect.", true),
            new Argument("amountPercent", typeof(float), "The amount of stamina percentage to add/set/remove.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1]!;
            float amount = (float)Arguments[2]!;

            Action<Player> action = Arguments[0]!.ToUpper() switch
            {
                "ADD" => player => { player.Stamina += amount; },
                "SET" => player => { player.Stamina = amount; },
                "REMOVE" => player => { player.Stamina -= amount; },
                _ => throw new ImpossibleException()
            };

            foreach (Player player in players)
            {
                action(player);
            }

            return new(true);
        }
    }
}