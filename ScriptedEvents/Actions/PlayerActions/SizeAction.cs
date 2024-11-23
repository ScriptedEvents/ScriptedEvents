using System;
using Exiled.API.Features;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class SizeAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Resize";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Sets provided players to the given size.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to resize.", true),
            new Argument("x", typeof(float), "The X size to put on the player.", true),
            new Argument("y", typeof(float), "The Y size to put on the player.", true),
            new Argument("z", typeof(float), "The Z size to put on the player.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            float x = (float)Arguments[1]!;
            float y = (float)Arguments[2]!;
            float z = (float)Arguments[3]!;

            foreach (Player player in (Player[])Arguments[0]!)
                player.Scale = new(x, y, z);

            return new(true);
        }
    }
}
