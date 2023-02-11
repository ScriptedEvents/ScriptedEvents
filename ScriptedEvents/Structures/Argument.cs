namespace ScriptedEvents.Structures
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using PlayerRoles;

    public class Argument
    {
        public Argument(string argumentName, Type type, string description, bool required)
        {
            ArgumentName = argumentName;
            Type = type;
            Description = description;
            Required = required;
        }

        public string ArgumentName { get; }
        public Type Type { get; }
        public string Description { get; }
        public bool Required { get; }

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
