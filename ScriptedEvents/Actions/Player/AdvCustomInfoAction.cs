﻿using System;
using Exiled.API.Extensions;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Player
{
    public class AdvCustomInfoAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ADVCUSTOMINFO";

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
            new Argument("targets", typeof(PlayerCollection), "The players that will see the action taking place. Only used if mode is SET.", false),
            new Argument("text", typeof(string), "The text to set custom info to. Only used if mode is SET.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            PlayerCollection plys = (PlayerCollection)Arguments[1];

            switch (Arguments[0].ToUpper())
            {
                case "SET":
                    if (Arguments.Length < 3) return new(MessageType.InvalidUsage, this, null, null, (object)ExpectedArguments);

                    string text = Arguments.JoinMessage(3)
                        .Replace("\\n", "\n")
                        .Replace("<br>", "\n");

                    PlayerCollection targetPlys = (PlayerCollection)Arguments[2];
                    foreach (Exiled.API.Features.Player ply in plys)
                    {
                        foreach (Exiled.API.Features.Player targetPlayer in targetPlys)
                        {
                            targetPlayer.SetPlayerInfoForTargetOnly(ply, text);
                        }
                    }

                    break;

                case "REMOVE":
                    foreach (Exiled.API.Features.Player ply in plys)
                    {
                        ply.CustomInfo = null;
                    }

                    break;
            }

            return new(true);
        }
    }
}
