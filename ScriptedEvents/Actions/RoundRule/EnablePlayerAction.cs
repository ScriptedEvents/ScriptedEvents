namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;

    using ScriptedEvents.API.Constants;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class EnablePlayerAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "ENABLEPLAYER";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Enables a feature for the entire round, but only for certain player(s).";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.DisableKeys;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to enable for.", true),
            new Argument("key", typeof(string), "The key of the feature to enable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetPlayers(Arguments[0], null, out PlayerCollection players, script))
                return new(false, players.Message);

            string key = Arguments[1].ToUpper();
            var rule = MainPlugin.Handlers.GetPlayerDisableRule(key);

            if (rule.HasValue)
            {
                List<Player> inner = players.GetInnerList();
                rule.Value.Players.RemoveAll(ply => inner.Contains(ply));
            }

            return new(true);
        }
    }
}
