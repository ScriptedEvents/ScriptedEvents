﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SetMaxHPAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "MAXHP";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Health;

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Set the Maximum HP of the targeted players.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("maxhealth", typeof(float), "The amount of max health to set the player to. Variables ARE supported.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection plys, script))
                return new(false, plys.Message);

            if (!VariableSystem.TryParse(Arguments[1], out float hp, script))
                return new(MessageType.NotANumber, this, "maxhealth", Arguments[1]);
            if (hp < 0)
                return new(MessageType.LessThanZeroNumber, this, "maxhealth", hp);

            foreach (Player ply in plys)
                ply.MaxHealth = hp;

            return new(true);
        }
    }
}
