namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using Respawning;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

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
            new Argument("team", typeof(SpawnableTeamType), "The spawn team (ChaosInsurgency or NineTailedFox).", true),
            new Argument("amount", typeof(int), "The amount to apply.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            SpawnableTeamType team = (SpawnableTeamType)Arguments[1];
            float amount = (float)Arguments[2];

            switch (Arguments[0].ToUpper())
            {
                case "ADD":
                    Respawn.GrantTickets(team, amount);
                    break;
                case "REMOVE":
                    Respawn.RemoveTickets(team, amount);
                    break;
                case "SET":
                    switch (team)
                    {
                        case SpawnableTeamType.ChaosInsurgency:
                            Respawn.ChaosTickets = amount;
                            break;
                        case SpawnableTeamType.NineTailedFox:
                            Respawn.NtfTickets = amount;
                            break;
                    }

                    break;
            }

            return new(true);
        }
    }
}
