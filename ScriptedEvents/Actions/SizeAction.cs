namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SizeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SIZE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets all players to the given size.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(List<Player>), "The players to rescale.", true),
            new Argument("size X", typeof(float), "The X size to put on the player.", true),
            new Argument("size Y", typeof(float), "The Y size to put on the player.", true),
            new Argument("size Z", typeof(float), "The Z size to put on the player.", true),
            new Argument("max", typeof(int), "The maximum amount of players to rescale (default: unlimited).", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!VariableSystem.TryParse(Arguments.ElementAt(1), out float x, script))
                return new(MessageType.NotANumber, this, "X", Arguments.ElementAt(1));

            if (!VariableSystem.TryParse(Arguments.ElementAt(2), out float y, script))
                return new(MessageType.NotANumber, this, "Y", Arguments.ElementAt(2));

            if (!VariableSystem.TryParse(Arguments.ElementAt(3), out float z, script))
                return new(MessageType.NotANumber, this, "Z", Arguments.ElementAt(3));

            int max = -1;

            if (Arguments.Length > 4)
            {
                if (!VariableSystem.TryParse(Arguments[4], out max, script))
                    return new(MessageType.NotANumber, this, "max", Arguments[4]);
                if (max < 0)
                    return new(MessageType.LessThanZeroNumber, this, "max", max);
            }

            if (!ScriptHelper.TryGetPlayers(Arguments[0], max, out PlayerCollection plys, script))
                return new(false, plys.Message);

            foreach (Player player in plys)
                player.Scale = new(x, y, z);

            return new(true);
        }
    }
}
