namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class SizeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SIZE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets all players to the given size.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to rescale.", true),
            new Argument("x", typeof(float), "The X size to put on the player.", true),
            new Argument("y", typeof(float), "The Y size to put on the player.", true),
            new Argument("z", typeof(float), "The Z size to put on the player.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 4) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            float x = (float)Arguments[1];
            float y = (float)Arguments[2];
            float z = (float)Arguments[3];

            foreach (Player player in (PlayerCollection)Arguments[0])
                player.Scale = new(x, y, z);

            return new(true);
        }
    }
}
