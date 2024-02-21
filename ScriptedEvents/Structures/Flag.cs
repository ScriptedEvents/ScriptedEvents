namespace ScriptedEvents.Structures
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a flag, a script component that modifies a script's behavior.
    /// </summary>
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

        /// <summary>
        /// Gets or sets the flag's key.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Gets or sets the flag's arguments.
        /// </summary>
        public string[] Arguments { get; set; }

        /// <inheritdoc/>
        public override readonly string ToString()
        {
            return $"!-- {Key} [{string.Join(" ", Arguments)}]";
        }
    }
}
