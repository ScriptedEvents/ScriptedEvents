using ScriptedEvents.API.Constants;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class EnablePlayerAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "EnablePlayerRule";

        /// <inheritdoc/>
        public string[] Aliases => new[] { "EnablePlrRule" };

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Enables a feature for the entire round, but only for certain player(s).";

        /// <inheritdoc/>
        public string LongDescription => ConstMessages.DisableKeys;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to enable for.", true),
            new Argument("key", typeof(string), "The key of the feature to enable. See documentation for a whole list of keys.", true),
        };

        public ActionResponse Execute(Script script)
        {
            PlayerCollection players = (PlayerCollection)Arguments[0]!;
            string key = Arguments[1]!.ToUpper();
            
            var rule = EventHandlingModule.Singleton!.GetPlayerDisableRule(key);
            if (!rule.HasValue) 
                return new(true);
            
            rule.Value.Players.RemoveAll(ply => players.GetInnerList().Contains(ply));

            return new(true);
        }
    }
}
