namespace ScriptedEvents.Structures
{
    using System;

    /// <summary>
    /// Contains information about the name of an action.
    /// </summary>
    public struct ActionNameData : IEquatable<ActionNameData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ActionNameData"/> struct.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="aliases">The aliases.</param>
        public ActionNameData(string name, string[] aliases)
        {
            Name = name;
            Aliases = aliases;
        }

        /// <summary>
        /// Gets or sets the name of the action.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the aliases of the action.
        /// </summary>
        public string[] Aliases { get; set; }

        public bool Equals(ActionNameData other)
        {
            return Name == other.Name && Aliases.Equals(other.Aliases);
        }

        public override bool Equals(object? obj)
        {
            return obj is ActionNameData other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Name.GetHashCode() * 397) ^ Aliases.GetHashCode();
            }
        }
    }
}
