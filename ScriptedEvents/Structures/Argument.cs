namespace ScriptedEvents.Structures
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using PlayerRoles;

    /// <summary>
    /// Represents an argument for an action.
    /// </summary>
    public class Argument
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="Argument"/> class.
        /// </summary>
        /// <param name="argumentName">The name of the argument.</param>
        /// <param name="type">The type of the argument.</param>
        /// <param name="description">The description of the argument.</param>
        /// <param name="required">Whether or not the argument is required.</param>
        public Argument(string argumentName, Type type, string description, bool required)
        {
            ArgumentName = argumentName;
            Type = type;
            Description = description;
            Required = required;
        }

        /// <summary>
        /// Gets the name of the argument.
        /// </summary>
        public string ArgumentName { get; }

        /// <summary>
        /// Gets the type of the argument.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets the description of the argument.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets a value indicating whether or not the argument is required.
        /// </summary>
        public bool Required { get; }

        /// <summary>
        /// Gets the <see cref="Type"/> in a human-readable form.
        /// </summary>
        public string TypeString
        {
            get
            {
                // Todo: turn into dictionary
                if (Type == typeof(string))
                {
                    return "String (Text)";
                }
                else if (Type == typeof(int))
                {
                    return "Int (Whole Number)";
                }
                else if (Type == typeof(float))
                {
                    return "Float (Number)";
                }
                else if (Type == typeof(bool))
                {
                    return "Boolean (TRUE/FALSE)";
                }
                else if (Type == typeof(List<Player>))
                {
                    return "Player List";
                }
                else if (Type == typeof(List<Door>))
                {
                    return "Door List";
                }
                else if (Type == typeof(RoleTypeId))
                {
                    return "RoleTypeId (Role Name/Number)";
                }
                else if (Type == typeof(object))
                {
                    return "Variable";
                }

                return Type.Name;
            }
        }
    }
}
