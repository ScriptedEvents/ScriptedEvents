﻿namespace ScriptedEvents.Actions
{
    using System;
    using Exiled.API.Features;
    using LightContainmentZoneDecontamination;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.Structures;

    public class DecontaminateAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DECONTAMINATE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Round;

        /// <inheritdoc/>
        public string Description => "Enables, disables, or forces decontamination.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The action (ENABLE, DISABLE, FORCE). Default: FORCE", false),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments[0].ToUpper())
            {
                case "DISABLE":
                    // Is there an Exiled API for this?
                    DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
                    break;
                case "ENABLE":
                    // And this?
                    DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.None;
                    break;
                case "FORCE":
                default:
                    Map.StartDecontamination();
                    break;
            }

            return new(true);
        }
    }
}