namespace ScriptedEvents.Actions.Map
{
    using System;
    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;
    using Exiled.API.Interfaces;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Features.Exceptions;
    using ScriptedEvents.Enums;
    using ScriptedEvents.Interfaces;
    using ScriptedEvents.Structures;

    public class DoorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "Door";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object?[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Manages facility doors. Keep in mind that some modes might not work with some doors.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new Option("Lock", "Lock doors."),
                new Option("Unlock", "Unlock doors."),
                new Option("Open", "Open doors."),
                new Option("Close", "Close doors."),
                new Option("Destroy", "Permanently destroy doors.")),
            new Argument("doors", typeof(Door[]), "The doors to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            var doors = (Door[])Arguments[1]!;

            Action<Door> action = Arguments[0]!.ToUpper() switch
            {
                "OPEN" => (door) => door.IsOpen = true,
                "CLOSE" => (door) => door.IsOpen = false,
                "LOCK" => (door) =>
                {
                    if (door.IsLocked) return;

                    door.ChangeLock(DoorLockType.AdminCommand);
                },
                "UNLOCK" => (door) => door.Unlock(),
                "DESTROY" => (door) =>
                {
                    if (door is IDamageableDoor { IsDestroyed: false } breaker) breaker.Break();
                },
                _ => throw new ImpossibleException()
            };

            foreach (Door door in doors)
            {
                action(door);
            }

            return new(true);
        }
    }
}