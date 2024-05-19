namespace ScriptedEvents.Actions
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Doors;
    using Exiled.API.Interfaces;

    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Extensions;
    using ScriptedEvents.API.Interfaces;
    using ScriptedEvents.Structures;

    public class DoorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DOOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] RawArguments { get; set; }

        /// <inheritdoc/>
        public object[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Controls map doors.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new OptionsArgument("mode", true,
                new("LOCK", "Lock doors."),
                new("UNLOCK", "Unlock doors."),
                new("OPEN", "Open doors."),
                new("CLOSE", "Close doors."),
                new("DESTROY", "Permanently destroy doors.")),
            new Argument("doors", typeof(Door[]), "The doors to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            Door[] doors = (Door[])Arguments[1];

            Action<Door> action = null;
            switch (Arguments[0].ToUpper())
            {
                case "OPEN":
                    action = (door) => door.IsOpen = true;
                    break;
                case "CLOSE":
                    action = (door) => door.IsOpen = false;
                    break;
                case "LOCK":
                    action = (door) =>
                    {
                        if (door.IsLocked)
                            return;

                        door.ChangeLock(DoorLockType.AdminCommand);
                    };
                    break;
                case "UNLOCK":
                    action = (door) => door.Unlock();
                    break;
                case "DESTROY":
                    action = (door) =>
                    {
                        if (door is IDamageableDoor breaker && !breaker.IsDestroyed)
                            breaker.Break();
                    };
                    break;
            }

            foreach (Door door in doors)
            {
                action(door);
            }

            return new(true);
        }
    }
}