using Exiled.API.Enums;
using Exiled.API.Features;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class OpenDoorAction : IAction
    {
        public string Name => "OPENDOOR";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 1) return new(false, "Missing arguments: doorType, duration(optional)");

            if (!ScriptHelper.TryGetDoors(Arguments.ElementAt(0), out List<Door> doors))
                return new(false, "Invalid door(s) provided!");

            foreach (var door in doors)
            {
                if (Arguments.Length == 2)
                {
                    bool previousState = door.IsOpen;
                    if (!ScriptHelper.TryConvertNumber(Arguments[1], out int duration))
                        return new(false, "Second argument must be an int or range of ints!");
                    Timing.CallDelayed(duration, () =>
                    {
                        door.IsOpen = previousState;
                    });
                }
                door.IsOpen = true;
            }
            return new(true, string.Empty);
        }
    }
}
