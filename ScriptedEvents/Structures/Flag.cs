namespace ScriptedEvents.Structures
{
    using System.Collections.Generic;
    using System.Linq;

    public struct Flag
    {
        public Flag(string key, IEnumerable<string> arguments)
        {
            Key = key;
            Arguments = arguments.ToArray();
        }

        public string Key { get; set; }

        public string[] Arguments { get; set; }

        public override string ToString()
        {
            return $"!-- {Key} [{string.Join(" ", Arguments)}]";
        }
    }
}
