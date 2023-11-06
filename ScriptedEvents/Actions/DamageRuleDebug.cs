namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DamageRuleDebug : IScriptAction, IHiddenAction
    {

        /// <inheritdoc/>
        public string Name { get; } = "DAMAGERULEDEBUG";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Misc;

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments[0] == "CLEAR")
            {
                MainPlugin.Handlers.DamageRules.Clear();
                return new(true);
            }

            Player attacker = Player.Get(Arguments[0]);
            Player receiver = Player.Get(Arguments[1]);

            foreach (var rule in MainPlugin.Handlers.DamageRules)
            {
                foreach (Player ply in Player.List)
                    ply.RemoteAdminMessage($"{attacker.DisplayNickname} will deal {rule.DetermineMultiplier(attacker, receiver)}x damage to {receiver.DisplayNickname} (TYPE: {rule.Type})");
            }

            return new(true);
        }
    }
}
