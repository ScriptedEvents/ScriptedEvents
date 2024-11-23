using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.PlayerActions
{
    public class CustomInfoAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "SetCustomInfo";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Sets the custom info of the targeted player(s) for everyone to see.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("players", typeof(Player[]), "The players to affect.", true),
            new Argument("text", typeof(string), "The text to set custom info to. Only used if mode is SET.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Player[] plys = (Player[])Arguments[0]!;
            string text = Arguments.JoinMessage(1)
                .Replace("\\n", "\n")
                .Replace("<br>", "\n");
            
            foreach (Player ply in plys)
            {
                ply.CustomInfo = text;
            }
            
            return new(true);
        }
    }
}
