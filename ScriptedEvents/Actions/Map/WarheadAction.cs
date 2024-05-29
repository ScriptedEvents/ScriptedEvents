namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Features;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class WarheadAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "WARHEAD";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Forces a specific warhead action.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("action", true,
                new("START", "Starts the warhead."),
                new("STOP", "Stops the warhead."),
                new("LOCK", "Prevents the warhead from being enabled."),
                new("UNLOCK", "Allows the warhead to be enabled again."),
                new("DETONATE", "Immediately detonates the warhead."),
                new("BLASTDOORS", "Closes the surface blast doors on top of the elevators. Doesn't start or detonate the warhead."),
                new("ARM", "Arms the warhead (switches lever to ON)."),
                new("DISARM", "Disarms the warhead (switches lever to OFF)."),
                new("OPEN", "Opens the keycard panel on the surface."),
                new("CLOSE", "Closes the keycard panel on the surface."),
                new("SHAKE", "Imitates an explosion without killing anyone.")),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
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
                case "STARTDETONATION":
                    Warhead.DetonationTimer = 10;
                    Warhead.Controller.InstantPrepare();
                    Warhead.Controller.StartDetonation(false, true);
                    break;
                case "SHAKE":
                    Warhead.Shake();
                    break;
            }

            return new(true);
        }
    }
}
