﻿using System;
using ScriptedEvents.API.Enums;
using ScriptedEvents.API.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Logic
{
    public class EndIfAction : IScriptAction, ILogicAction, IHelpInfo, IIgnoresIfActionBlock
    {
        /// <inheritdoc/>
        public string Name => "ENDIF";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Logic;

        /// <inheritdoc/>
        public string Description => "Removes the action ignoring status enabled by the IF action, if one exists.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => Array.Empty<Argument>();

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            script.IfActionBlocksExecution = false;
            return new(true);
        }
    }
}