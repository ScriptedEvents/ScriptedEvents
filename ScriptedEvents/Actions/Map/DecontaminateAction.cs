namespace ScriptedEvents.Actions.Map
{
    using System;
    using LightContainmentZoneDecontamination;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class DecontaminateAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Decontaminate";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Manages LCZ decontamination.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Disable", "Disables the decontamination."),
                new Option("Enable", "Allows for decontamination"),
                new Option("Force", "Forces the decontamination to happen asap.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments[0]!.ToUpper())
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