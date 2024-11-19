﻿using System;
using PlayerRoles;
using ScriptedEvents.API.Modules;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.RoundRule
{
    public class DeleteInfectRule : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DelInfectRule";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.RoundRule;

        /// <inheritdoc/>
        public string Description => "Delete a currently-existing infection rule.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("role", typeof(RoleTypeId), "The role a player must die as to be infected.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            RoleTypeId oldRole = (RoleTypeId)Arguments[0]!;

            EventHandlingModule.Singleton!.InfectionRules.RemoveAll(rule => rule.OldRole == oldRole);

            return new(true);
        }
    }
}
