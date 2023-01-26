using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScriptedEvents.Actions
{
    public class BroadcastAction : IAction
    {
        public string Name => "BROADCAST";

        public string[] Aliases => Array.Empty<string>();

        public string[] Arguments { get; set; }

        public ActionResponse Execute()
        {
            if (Arguments.Length < 2) return new(false, "Missing argument: duration, message");
            if (!ScriptHelper.TryConvertNumber(Arguments[0], out int duration))
                return new(false, "First argument must be an int or range of ints!");

            string message = string.Join(" ", Arguments.Skip(1));
            Map.Broadcast((ushort)duration, message);
            return new(true);
        }
    }
}
