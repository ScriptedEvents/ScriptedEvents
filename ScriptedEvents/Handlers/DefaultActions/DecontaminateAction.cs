using Exiled.API.Features;
using LightContainmentZoneDecontamination;
using ScriptedEvents.API.Features.Actions;
using System;

namespace ScriptedEvents.Actions
{
    public class DecontaminateAction : IScriptAction
    {
        public string Name => "DECONTAMINATE";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

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