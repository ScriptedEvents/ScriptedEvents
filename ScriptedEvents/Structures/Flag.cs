namespace ScriptedEvents.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public struct Flag
    {
        public Flag(string key, IEnumerable<string> arguments)
        {
            Key = key;

            if (arguments is null)
            {
                Arguments = Array.Empty<string>();
                return;
            }

            Arguments = arguments.Select(s => s.Trim()).ToArray();
        }

        public string Key { get; set; }

        public string[] Arguments { get; set; }

        public override string ToString()
        {
            return $"!-- {Key} [{string.Join(" ", Arguments)}]";
        }
    }
}
