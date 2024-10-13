namespace ScriptedEvents.Actions.MapActions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using MEC;

    using PlayerRoles;

    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    using Tesla = Exiled.API.Features.TeslaGate;

    // Todo: Needs reworked entirely
    public class TeslaAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "TESLA";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Modifies tesla gates.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Detect", "Make tesla gates detect specified players."),
                new("Ignore", "Make tesla gates ignore specified players.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            return new(false, "Action has no implementation.");
        }
    }
}
