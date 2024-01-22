namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class WarheadAction : IScriptAction, IHelpInfo, ILongDescription
    {
        /// <inheritdoc/>
        public string Name => "WARHEAD";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Forces a specific warhead action.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("action", typeof(string), "The action to run.", true),
        };

        /// <inheritdoc/>
        public string LongDescription { get; } = @$"Valid options:
- START - Starts the warhead (even if it is disarmed or on cooldown)
- STOP - Stops the warhead
- LOCK - Prevents the warhead from being enabled
- UNLOCK Allows the warhead to be enabled again
- DETONATE - Detonates the warhead immediately
- BLASTDOORS - Closes the surface blast doors, doesn't start or detonate the warhead
- ARM - Arms the warhead (switches lever to ON)
- DISARM - Disarms the warhead (switches lever to OFF)
- OPEN - Opens the keycard panel on the surface
- CLOSE - Closes the keycard panel on the surface";

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 1) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            switch (Arguments[0].ToUpper())
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
// Jraylor Zone
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
// Jraylor Zone
                case "BLASTDOORS":
                    Warhead.CloseBlastDoors();
                    break;
                default:
                    return new(MessageType.InvalidOption, this, "action", Arguments[0], "START/STOP/DETONATE/LOCK/UNLOCK/BLASTDOORS/ARM/DISARM/OPEN/CLOSE");
            }

            return new(true);
        }
    }
}
