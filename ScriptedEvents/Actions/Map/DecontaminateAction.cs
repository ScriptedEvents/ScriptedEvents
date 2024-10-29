using ScriptedEvents.Interfaces;

namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using LightContainmentZoneDecontamination;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Structures;

    public class DecontaminateAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DECONTAMINATE";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Enables, disables, or forces decontamination.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("Disable", "Disables the decontamination."),
                new("Enable", "Allows for decontamination"),
                new("Force", "Forces the decontamination to happen asap.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments[0].ToUpper())
            {
                case "DISABLE":
                    // Todo: Is there an Exiled API for this?
                    DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
                    break;
                case "ENABLE":
                    // Todo: And this?
                    DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.None;
                    break;
                default:
                    Exiled.API.Features.Map.StartDecontamination();
                    break;
            }

            return new(true);
        }
    }
}