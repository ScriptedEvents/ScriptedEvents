using System;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class WarheadAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Warhead";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Forces a specific warhead action.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("action", true,
                new Option("Start", "Starts the warhead."),
                new Option("Stop", "Stops the warhead."),
                new Option("Lock", "Prevents the warhead from being enabled."),
                new Option("Unlock", "Allows the warhead to be enabled again."),
                new Option("Detonate", "Immediately detonates the warhead."),
                new Option("BlastDoors", "Closes the surface blast doors on top of the elevators. Doesn't start or detonate the warhead."),
                new Option("Arm", "Arms the warhead (switches lever to ON)."),
                new Option("Disarm", "Disarms the warhead (switches lever to OFF)."),
                new Option("Open", "Opens the keycard panel on the surface."),
                new Option("Close", "Closes the keycard panel on the surface."),
                new Option("Shake", "Imitates an explosion without killing anyone.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            switch (Arguments[0]!.ToUpper())
            {
                case "START":
                    Warhead.Start();
                    break;
                case "STOP":
                    Warhead.Stop();
                    break;
                case "DETONATE":
                    Warhead.Detonate();
                    break;
                case "LOCK":
                    Warhead.IsLocked = true;
                    break;
                case "UNLOCK":
                    Warhead.IsLocked = false;
                    break;
                case "ARM":
                    Warhead.LeverStatus = true;
                    break;
                case "DISARM":
                    Warhead.LeverStatus = false;
                    break;
                case "OPEN":
                    Warhead.IsKeycardActivated = true;
                    break;
                case "CLOSE":
                    Warhead.IsKeycardActivated = false;
                    break;
                case "BLASTDOORS":
                    Warhead.CloseBlastDoors();
                    break;
                case "SHAKE":
                    Warhead.Shake();
                    break;
            }

            return new(true);
        }
    }
}
