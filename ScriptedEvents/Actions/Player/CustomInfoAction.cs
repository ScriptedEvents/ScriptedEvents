﻿using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Player
{
    public class CustomInfoAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "CUSTOMINFO";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Player;

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public string Description => "Sets/clears the custom info of the targeted player(s).";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SET", "Sets players' custom info."),
                new("REMOVE", "Clears players' custom info.")),
            new Argument("players", typeof(PlayerCollection), "The players to affect.", true),
            new Argument("text", typeof(string), "The text to set custom info to. Only used if mode is SET.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[1];

            string mode = Arguments[0].ToUpper();
            switch (mode)
            {
                case "SET":
                    if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, null, (object)ExpectedArguments);
                    string text = Arguments.JoinMessage(2)
                        .Replace("\\n", "\n")
                        .Replace("<br>", "\n");
                    foreach (Exiled.API.Features.Player ply in plys)
                    {
                        ply.CustomInfo = text;
                    }

                    break;

                case "REMOVE":
                    foreach (Exiled.API.Features.Player ply in plys)
                    {
                        ply.CustomInfo = string.Empty;
                    }

                    break;
            }

            return new(true);
        }
    }
}
