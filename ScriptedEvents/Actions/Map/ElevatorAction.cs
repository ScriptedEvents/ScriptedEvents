namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Doors;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ElevatorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ELEVATOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Controls map elevators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (SEND/LOCK/UNLOCK).", true),
            new Argument("elevators", typeof(List<>), "The elevators to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetLifts(Arguments[1], out Lift[] lifts, script))
                return new(false, "Invalid lift(s) provided!");

            Action<Lift> action;
            switch (Arguments[0].ToUpper())
            {
                case "SEND":
                    action = (lift) => lift.TryStart(lift.CurrentLevel == 0 ? 1 : 0, true);
                    break;

                case "LOCK":
                    action = (lift) =>
                    {
                        if (lift.IsLocked)
                        {
                            return;
                        }

                        foreach (var door in lift.Doors)
                        {
                            door.ChangeLock(DoorLockType.AdminCommand);
                            lift.Base.RefreshLocks(lift.Group, door.Base);
                        }
                    };
                    break;

                case "UNLOCK":
                    action = (lift) =>
                    {
                        if (!lift.IsLocked)
                        {
                            return;
                        }

                        foreach (var door in lift.Doors)
                        {
                            door.DoorLockType = DoorLockType.None;
                            door.ChangeLock(DoorLockType.None);

                            lift.Base.RefreshLocks(lift.Group, door.Base);
                        }
                    };
                    break;

                default:
                    return new(MessageType.InvalidOption, this, "mode", Arguments[0], "SEND/LOCK/UNLOCK");
            }

            foreach (Lift lift in lifts)
            {
                action(lift);
            }

            return new(true);
        }
    }
}