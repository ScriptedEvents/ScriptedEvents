namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class ElevatorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "ELEVATOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Controls map elevators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("SEND", "Moves the elevator."),
                new("LOCK", "Locks the elevator, preventing it from being used."),
                new("UNLOCK", "Unlocks the previously-locked elevator, allowing it to be used again.")),
            new Argument("elevators", typeof(Lift[]), "The elevators to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Lift[] lifts = (Lift[])Arguments[1];

            Action<Lift> action = null;
            switch (Arguments[0].ToUpper())
            {
                case "SEND":
                    action = (lift) => lift.TryStart(lift.CurrentLevel == 0 ? 1 : 0, true);
                    break;

                case "LOCK":
                    action = (lift) =>
                    {
                        if (lift.IsLocked)
                            return;

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
            }

            foreach (Lift lift in lifts)
            {
                action(lift);
            }

            return new(true);
        }
    }
}