using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class LockDoorAction : IAction
    {
        public string Name => "LOCKDOOR";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 2) return new(false, "Missing arguments: doorType, duration");

            if (!ScriptHelper.TryGetDoors(Arguments.ElementAt(0), out List<Door> doors))
                return new(false, "Invalid door(s) provided!");
            if (!ScriptHelper.TryConvertNumber(Arguments[1], out int duration))
                return new(false, "Second argument must be an int or range of ints!");

            foreach (var door in doors)
            {
                door.Lock(duration, Exiled.API.Enums.DoorLockType.AdminCommand);
            }
            return new(true, string.Empty);
        }
    }
}
