namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DisablePlayerAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "DISABLEPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Disables a feature for the entire round, but only for certain player(s).";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.DisableKeys;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to disable for.", true),
            new Argument("key", typeof(string), "The key of the feature to disable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0];

            string key = Arguments[1].ToUpper();
            var rule = MainPlugin.Handlers.GetPlayerDisableRule(key);

            if (rule.HasValue)
            {
                rule.Value.Players.AddRange(players.GetInnerList());
            }
            else
            {
                MainPlugin.Handlers.DisabledPlayerKeys.Add(new(key, players.GetInnerList()));
            }

            return new(true);
        }
    }
}
