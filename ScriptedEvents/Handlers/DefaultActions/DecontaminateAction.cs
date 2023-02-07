using Exiled.API.Features;
using LightContainmentZoneDecontamination;
using ScriptedEvents.API.Features.Actions;
using System;

namespace ScriptedEvents.Actions
{
    public class DecontaminateAction : IAction
    {
        public string Name => "DECONTAMINATE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1)
                return new(false, "Missing argument: action (FORCE/DISABLE/ENABLE)");

            switch (Arguments[0].ToUpper())
            {
                case "FORCE":
                    Map.StartDecontamination();
                    break;
                case "DISABLE":
                    // Is there an Exiled API for this?
                    DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
                    break;
                case "ENABLE":
                    // And this?
                    DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.None;
                    break;
                default:
                    return new(false, "First argument must be FORCE/DISABLE/ENABLE!");
            }

            return new(true);
        }
    }
}