using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using ScriptedEvents.API.Features.Actions;
using ScriptedEvents.API.Helpers;
using ScriptedEvents.Handlers.Variables;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ScriptedEvents.Handlers.DefaultActions
{
    public class DoorAction : IScriptAction, IHelpInfo
    {
        public string Name => "DOOR";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public string Description => "Controls map doors.";

        public Argument[] ExpectedArguments => new[]
        {
            new Argument("mode", typeof(string), "The mode (LOCK, UNLOCK, OPEN, CLOSE, DESTROY).", true),
            new Argument("doors", typeof(List<Door>), "The doors to affect.", true),
            new Argument("duration", typeof(float), "The duration. Leave blank for indefinite duration. Variables & Math are supported.", false)
        };

        public ActionResponse Execute(Script script)
        {
            if (Arguments.Length < 2) return new(false, "Missing arguments: LOCK/UNLOCK/OPEN/CLOSE/DESTROY, doorType, duration(optional)");

            if (!ScriptHelper.TryGetDoors(Arguments[1], out List<Door> doors))
                return new(false, "Invalid door(s) provided!");

            float duration = 0;
            if (Arguments.Length > 2)
            {
                string formula = ConditionVariables.ReplaceVariables(string.Join(" ", Arguments.Skip(2)));

                try
                {
                    duration = (float)ConditionHelper.Math(formula);
                }
                catch (Exception ex)
                {
                    return new(false, $"Invalid duration condition provided! Condition: {formula} Error type: '{ex.GetType().Name}' Message: '{ex.Message}'.");
                }
            }

            Action<Door> action;
            Action<Door, float> revertAction;
            switch (Arguments[0].ToUpper())
            {
                case "OPEN":
                    action = (door) => door.IsOpen = true;
                    revertAction = RevertOpened;
                    break;
                case "CLOSE":
                    action = (door) => door.IsOpen = false;
                    revertAction = RevertOpened;
                    break;
                case "LOCK":
                    action = (door) => door.Lock(duration, DoorLockType.AdminCommand);
                    revertAction = RevertLock;
                    break;
                case "UNLOCK":
                    action = (door) => door.Unlock();
                    revertAction = RevertLock;
                    break;
                case "DESTROY":
                    action = (door) => door.BreakDoor();
                    revertAction = null;
                    break;
                default:
                    return new(false, "First argument must be OPEN/CLOSE/LOCK/UNLOCK/DESTROY!");
            }

            foreach (Door door in doors)
            {
                if (duration != 0 && revertAction != null)
                        revertAction(door, duration);

                action(door);
            }
            return new(true, string.Empty);
        }

        public void RevertOpened(Door door, float duration)
        {
            bool state = door.IsOpen;
            Timing.CallDelayed(duration, () => door.IsOpen = state);
        }

        public void RevertLock(Door door, float duration)
        {
            DoorLockType state = door.DoorLockType;
            Timing.CallDelayed(duration, () => door.ChangeLock(state));
        }
    }
}