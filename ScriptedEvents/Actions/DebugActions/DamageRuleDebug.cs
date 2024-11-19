using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    public class DamageRuleDebug : IScriptAction, IHiddenAction
    {
        /// <inheritdoc/>
        public string Name { get; } = "DAMAGERULEDEBUG";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Debug;

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("input1", typeof(string), string.Empty, true),
            new Argument("input2", typeof(string), string.Empty, false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments[0].ToUpper() == "CLEAR")
            {
                EventHandlingModule.Singleton!.DamageRules.Clear();
                return new(true);
            }

            Player attacker = Player.Get((string)Arguments[0]);
            Player receiver = Player.Get((string)Arguments[1]);

            foreach (var rule in EventHandlingModule.Singleton!.DamageRules)
            {
                foreach (Player ply in Player.List)
                    ply.RemoteAdminMessage($"{attacker.DisplayNickname} will deal {rule.DetermineMultiplier(attacker, receiver)}x damage to {receiver.DisplayNickname} (TYPE: {rule.Type})");
            }

            return new(true);
        }
    }
}
