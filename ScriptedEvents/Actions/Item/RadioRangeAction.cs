using ScriptedEvents.API.Modules;

namespace ScriptedEvents.Actions.Item
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class RadioRangeAction : IScriptAction, IHelpInfo, ISampleAction
    {
        /// <inheritdoc/>
        public string Name => "RadioRange";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Item;

        /// <inheritdoc/>
        public string Description => "Modifies radio range settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Set", "Only sets the range of the radio."),
                new Option("Lock", "Sets the range and locks its value so it cannot be changed.")),
            new Argument("players", typeof(PlayerCollection), "The players to change the radio settings of.", true),
            new Argument("range", typeof(RadioRange), "The new radio range. Must be: Short, Medium, Long, or Ultra", true),
        };

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new RadioRangeSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var mode = Arguments[0]!.ToUpper();
            var players = (PlayerCollection)Arguments[1]!;
            var range = (RadioRange)Arguments[2]!;

            foreach (Player ply in players)
            {
                foreach (var item in ply.Items)
                {
                    if (item is not Radio radio) continue;
                    radio.Range = range;
                    radio.Base.SendStatusMessage();
                }
            }

            if (mode is not "LOCK") return new(true);

            foreach (Player ply in players)
            {
                EventHandlingModule.Singleton!.LockedRadios[ply] = range;
            }

            return new(true);
        }
    }
}
