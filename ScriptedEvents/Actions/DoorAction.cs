﻿namespace ScriptedEvents.Actions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Enums;
    using Exiled.API.Features;
    using MEC;
    using ScriptedEvents.Actions.Interfaces;
    using ScriptedEvents.API.Enums;
    using ScriptedEvents.API.Features;
    using ScriptedEvents.Structures;
    using ScriptedEvents.Variables;

    public class DoorAction : IScriptAction, IHelpInfo
    {
        /// <inheritdoc/>
        public string Name => "DOOR";

        /// <inheritdoc/>
        public string[] Aliases => Array.Empty<string>();

        /// <inheritdoc/>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public ActionSubgroup Subgroup => ActionSubgroup.Map;

        /// <inheritdoc/>
        public string Description => "Controls map doors.";

        /// <inheritdoc/>
        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (LOCK, UNLOCK, OPEN, CLOSE, DESTROY).", true),
            new Argument("doors", typeof(List<Door>), "The doors to affect.", true),
        };

        /// <inheritdoc/>
        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(MessageType.InvalidUsage, this, null, (object)ExpectedArguments);

            if (!ScriptHelper.TryGetDoors(Arguments[1], out Door[] doors))
                return new(false, "Invalid door(s) provided!");

            Action<Door> action;
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
                    action = (door) => door.BreakDoor();
                    break;
                default:
                    return new(MessageType.InvalidOption, this, "mode", Arguments[0], "OPEN/CLOSE/LOCK/UNLOCK/DESTROY");
            }

            foreach (Door door in doors)
            {
                action(door);
            }

            return new(true);
        }
    }
}