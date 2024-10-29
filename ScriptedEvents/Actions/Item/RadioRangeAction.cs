﻿using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions.Item
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using Actions.Samples.Interfaces;
    using Actions.Samples.Providers;
    using API.Enums;
    using API.Extensions;
    using Structures;

    public class RadioRangeAction : IScriptAction, IHelpInfo, ISampleAction
    {
        /// <inheritdoc/>
        public string Name => "RADIORANGE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Modifies radio range settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Set", "Only sets the range of the radio."),
                new("Lock", "Sets the range and locks its value so it cannot be changed.")),
            new Argument("players", typeof(PlayerCollection), "The players to change the radio settings of.", true),
            new Argument("range", typeof(RadioRange), "The new radio range. Must be: Short, Medium, Long, or Ultra", true),
        };

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new RadioRangeSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[1];
            RadioRange range = (RadioRange)Arguments[2];

            foreach (Player ply in players)
            {
                foreach (Item item in ply.Items)
                {
                    if (item is Radio radio)
                    {
                        radio.Range = range;
                        radio.Base.SendStatusMessage();
                    }
                }
            }

            if (Arguments[0].ToUpper() is "LOCK")
            {
                foreach (Player ply in players)
                {
                    MainPlugin.Handlers.LockedRadios[ply] = range;
                }
            }

            return new(true);
        }
    }
}
