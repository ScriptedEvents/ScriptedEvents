using System;
using Exiled.API.Extensions;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class AdvCustomInfoAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "AdvSetCustomInfo";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Sets the custom info of the specified player(s).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("targets", typeof(PlayerCollection), "The players that will see the action taking place.", true),
            new Argument("text", typeof(string), "The custom info content.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[0]!;
            PlayerCollection targetPlys = (PlayerCollection)Arguments[1]!;
            string text = Arguments.JoinMessage(2)
                .Replace("\\n", "\n")
                .Replace("<br>", "\n");
            
            foreach (Exiled.API.Features.Player ply in plys)
            {
                foreach (Exiled.API.Features.Player targetPlayer in targetPlys)
                {
                    targetPlayer.SetPlayerInfoForTargetOnly(ply, text);
                }
            }
            
            return new(true);
        }
    }
}
