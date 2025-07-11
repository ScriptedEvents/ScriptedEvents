﻿using System;
using Exiled.API.Features;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Server
{
    public class LogAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "LOG";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Server;

        /// <inheritdoc/>
        public string Description => "Creates a server console log.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("message", typeof(string), "The message.", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Log.Info(Arguments.JoinMessage(0));
            return new(true);
        }
    }
}