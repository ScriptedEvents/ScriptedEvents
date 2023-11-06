namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using Respawning;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class TicketAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TICKET";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Modifies tickets.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The action (ADD, REMOVE, SET).", true),
            new Argument("team", typeof(SpawnableTeamType), "The spawn team (ChaosInsurgency or NineTailedFox).", true),
            new Argument("amount", typeof(int), "The amount to apply. Variables are supported.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse<SpawnableTeamType>(Arguments[1], out SpawnableTeamType team, script))
                return new(false, "Invalid spawnable role provided. Must be ChaosInsurgency or NineTailedFox.");

            if (!VariableSystem.TryParse(Arguments[2], out float amount, script))
                return new(MessageType.NotANumber, this, "amount", Arguments[1]);

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
                default:
                    return new(MessageType.InvalidOption, this, "mode", "ADD/REMOVE/SET");
            }

            return new(true);
        }
    }
}
