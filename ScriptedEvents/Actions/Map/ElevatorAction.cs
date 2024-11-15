using System;
using Exiled.API.Enums;
using Exiled.API.Features;
using ScriptedEvents.API.Extensions;
using ScriptedEvents.API.Features.Exceptions;
using ScriptedEvents.Enums;
using ScriptedEvents.Interfaces;
using ScriptedEvents.Structures;

namespace ScriptedEvents.Actions.Map
{
    public class ElevatorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Elevator";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Controls facility elevators.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Send", "Moves the elevator."),
                new Option("Lock", "Locks the elevator, preventing it from being used."),
                new Option("Unlock", "Unlocks the previously-locked elevator, allowing it to be used again.")),
            new Argument("elevators", typeof(Lift[]), "The elevators to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Action<Lift> action = Arguments[0]!.ToUpper() switch
            {
                "SEND" => lift => lift.TryStart(lift.CurrentLevel == 0 ? 1 : 0, true),
                "LOCK" => lift =>
                {
                    if (lift.IsLocked) return;

                    foreach (var door in lift.Doors)
                    {
                        door.ChangeLock(DoorLockType.AdminCommand);
                        lift.Base.RefreshLocks(lift.Group, door.Base);
                    }
                },
                "UNLOCK" => lift =>
                {
                    if (!lift.IsLocked) return;

                    foreach (var door in lift.Doors)
                    {
                        door.DoorLockType = DoorLockType.None;
                        door.ChangeLock(DoorLockType.None);

                        lift.Base.RefreshLocks(lift.Group, door.Base);
                    }
                },
                _ => throw new ImpossibleException()
            };

            foreach (Lift lift in (Lift[])Arguments[1]!)
            {
                action(lift);
            }

            return new(true);
        }
    }
}