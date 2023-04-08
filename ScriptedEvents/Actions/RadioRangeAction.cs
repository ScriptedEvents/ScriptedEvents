namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.Actions.Samples.Interfaces;
    using ScriptedEvents.Actions.Samples.Providers;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Helpers;
    using ScriptedEvents.Structures;

    public class RadioRangeAction : IScriptAction, IHelpInfo, ISampleAction
    {
        /// <inheritdoc/>
        public string Name => "RADIORANGE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string Description => "Modifies radio range settings.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode to run. Valid options: SET, LOCK", true),
            new Argument("players", typeof(Player[]), "The players to change the radio settings of.", true),
            new Argument("range", typeof(RadioRange), "The new radio range. Must be: Short, Medium, Long, or Ultra", true),
        };

        /// <inheritdoc/>
        public ISampleProvider Samples { get; } = new RadioRangeSamples();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 3)
                return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[1], null, out Player[] players, script))
                return new(MessageType.NoPlayersFound, this, "players");

            if (!Enum.TryParse(Arguments[2], true, out RadioRange range))
                return new(false, "Invalid radio range provided. Must be: Short, Medium, Long, Ultra.");

            if (Arguments[0] is "LOCK" or "SET")
            {
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
            }
            else
            {
                return new(MessageType.InvalidOption, this, "mode", Arguments[0], "SET/LOCK");
            }

            if (Arguments[0] is "LOCK")
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
