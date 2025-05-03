using System;
using Exiled.API.Features;
using PlayerRoles;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Round
{
    public class TicketAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TICKET";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Modifies tickets.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("ADD", "Adds tickets to a team."),
                new("REMOVE", "Removes tickets from a team."),
                new("SET", "Set's a team's ticket amount.")),
            new Argument("faction", typeof(Faction), "The spawn team (ChaosInsurgency or NineTailedFox).", true),
            new Argument("amount", typeof(int), "The amount to apply.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Faction team = (Faction)Arguments[1];
            int amount = (int)Arguments[2];

            switch (Arguments[0].ToUpper())
            {
                case "ADD":
                    Respawn.GrantTokens(team, amount);
                    break;
                case "REMOVE":
                    Respawn.RemoveTokens(team, amount);
                    break;
                case "SET":
                    switch (team)
                    {
                        case Faction.FoundationEnemy:
                            Respawn.SetTokens(Exiled.API.Enums.SpawnableFaction.ChaosMiniWave, amount);
                            Respawn.SetTokens(Exiled.API.Enums.SpawnableFaction.ChaosWave, amount);
                            break;
                        case Faction.FoundationStaff:
                            Respawn.SetTokens(Exiled.API.Enums.SpawnableFaction.NtfMiniWave, amount);
                            Respawn.SetTokens(Exiled.API.Enums.SpawnableFaction.NtfWave, amount);
                            break;
                    }

                    break;
            }

            return new(true);
        }
    }
}
